using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using EscapeFromTheWoods.Database;

namespace EscapeFromTheWoods
{
    public class Wood
    {
        private const int drawingFactor = 8;
        private string path;
        private MongoDBRepository repo;
        private Random r = new Random(1);
        public int woodID { get; set; }
        public Dictionary<int, Tree> trees { get; set; }
        public List<Monkey> monkeys { get; private set; }
        private Map map;
        public Wood(int woodID, Dictionary<int, Tree> trees, Map map, string path, MongoDBRepository repo)
        {
            this.woodID = woodID;
            this.trees = trees;
            this.monkeys = new List<Monkey>();
            this.map = map;
            this.path = path;
            this.repo = repo;
        }
        public void PlaceMonkey(string monkeyName, int monkeyID)
        {
            int treeNr;
            int treeID;
            do
            {
                treeNr = r.Next(0, trees.Count - 1);
                treeID = trees.ElementAt(treeNr).Value.treeID;
            }
            while (trees[treeID].hasMonkey);
            Monkey m = new Monkey(monkeyID, monkeyName, trees[treeID]);
            monkeys.Add(m);
            trees[treeID].hasMonkey = true;
        }
        public void Escape()
        {
            List<List<Tree>> routes = new List<List<Tree>>();
            foreach (Monkey m in monkeys)
            {
                routes.Add(EscapeMonkey(m));
            }                
           WriteEscaperoutesToBitmap(routes);           
        }

        //logging
        private void writeRouteToDB(Monkey monkey,List<Tree> route)
        {
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} start");
            List<DBMonkeyRecord> records = new List<DBMonkeyRecord>();
            for (int j = 0; j < route.Count; j++)
            {
                records.Add(new DBMonkeyRecord(monkey.monkeyID, monkey.name, woodID,j, route[j].treeID, route[j].x, route[j].y));
            }
            DBMonkeyRecordSet set = new(records);
            List<DBMonkeyRecordSet> setList = new List<DBMonkeyRecordSet>() { set };
            repo.WriteMonkeyRecords(setList);
            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine($"{woodID}:write db routes {woodID},{monkey.name} end");
        }       
        public void WriteEscaperoutesToBitmap(List<List<Tree>> routes)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} start");
            Color[] cvalues = new Color[] { Color.Red, Color.Yellow, Color.Blue, Color.Cyan, Color.GreenYellow };
            Bitmap bm = new Bitmap((map.xmax - map.xmin) * drawingFactor, (map.ymax - map.ymin) * drawingFactor);
            Graphics g = Graphics.FromImage(bm);
            int delta = drawingFactor / 2;
            Pen p = new Pen(Color.Green, 1);
            foreach (Tree t in trees.Values)
            {
                g.DrawEllipse(p, t.x * drawingFactor, t.y * drawingFactor, drawingFactor, drawingFactor);
            }
            int colorN = 0;
            foreach (List<Tree> route in routes)
            {
                int p1x = route[0].x * drawingFactor + delta;
                int p1y = route[0].y * drawingFactor + delta;
                Color color = cvalues[colorN % cvalues.Length];
                Pen pen = new Pen(color, 1);
                g.DrawEllipse(pen, p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                g.FillEllipse(new SolidBrush(color), p1x - delta, p1y - delta, drawingFactor, drawingFactor);
                for (int i = 1; i < route.Count; i++)
                {
                    g.DrawLine(pen, p1x, p1y, route[i].x * drawingFactor + delta, route[i].y * drawingFactor + delta);
                    p1x = route[i].x * drawingFactor + delta;
                    p1y = route[i].y * drawingFactor + delta;
                }
                colorN++;
            }
            bm.Save(Path.Combine(path, woodID.ToString() + "_escapeRoutes.jpg"), ImageFormat.Jpeg);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"{woodID}:write bitmap routes {woodID} end");
        }        
        public void WriteWoodToDB()
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} start");
            List<DBWoodRecord> records = new List<DBWoodRecord>();
            foreach(Tree t in trees.Values)
            {
                records.Add(new DBWoodRecord(woodID, t.treeID,t.x,t.y));
            }
            DBWoodRecordSet set = new(records);
            List<DBWoodRecordSet> setList = new List<DBWoodRecordSet>() { set };
            repo.WriteWoodRecords(setList);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"{woodID}:write db wood {woodID} end");
        }
        public List<Tree> EscapeMonkey(Monkey monkey)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"{woodID}:start {woodID},{monkey.name}");
            Dictionary<int, bool> visited = trees.Keys.ToDictionary(key => key, _ => false);
            List<Tree> route = new List<Tree>() { monkey.tree };
            do
            {
                visited[monkey.tree.treeID] = true;
                SortedList<double, List<Tree>> distanceToMonkey = new SortedList<double, List<Tree>>();

                //zoek dichtste boom die nog niet is bezocht
                //TODO: gaat elke boom af. Kan efficienter door te limiteren welke bomen hij doorzoekt. (grids)
                foreach (Tree t in trees.Values)
                {
                    if ((!visited[t.treeID]) && (!t.hasMonkey))
                    {
                        double d = Math.Sqrt(Math.Pow(t.x - monkey.tree.x, 2) + Math.Pow(t.y - monkey.tree.y, 2));
                        if (distanceToMonkey.ContainsKey(d)) distanceToMonkey[d].Add(t);
                        else distanceToMonkey.Add(d, new List<Tree>() { t });
                    }
                }
                //distance to border
                //noord oost zuid west
                double distanceToBorder = (
                    new List<double>(){ 
                        map.ymax - monkey.tree.y,
                        map.xmax - monkey.tree.x,
                        monkey.tree.y-map.ymin,
                        monkey.tree.x-map.xmin })
                        .Min();
                if (distanceToMonkey.Count == 0)
                {
                    writeRouteToDB(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }
                if (distanceToBorder < distanceToMonkey.First().Key){
                    writeRouteToDB(monkey, route);
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.WriteLine($"{woodID}:end {woodID},{monkey.name}");
                    return route;
                }

                route.Add(distanceToMonkey.First().Value.First());
                monkey.tree = distanceToMonkey.First().Value.First();
            }
            while (true);
        }
    }
}
