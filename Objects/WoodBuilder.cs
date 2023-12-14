using EscapeFromTheWoods.Database;
using EscapeFromTheWoods.Objects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EscapeFromTheWoods
{
    public static class WoodBuilder
    {        
        //REFACTOR: Map parameter vervangen door TreesGrid
        public static Wood GetWood(int size, Map map, string path,MongoDBRepository repo, double delta)
        {
            Random r = new Random(100);
            Dictionary<int, Tree> trees = new Dictionary<int, Tree>();
            int n = 0;
            //TODO: bomen aanmaken kan ook verdeeld worden in een grid? Minimale refactoring, maar kan mss snelheid opleveren op grotere schaal?
            while(n<size)
            {
                Tree t = new Tree(IDgenerator.GetTreeID(),r.Next(map.xmin, map.xmax),r.Next(map.ymin, map.ymax));
                if (!trees.ContainsKey(t.treeID)) { trees.Add(t.treeID, t); n++; }
            }
            Wood w = new Wood(IDgenerator.GetWoodID(),trees,path,repo);
            TreesGrid grid = new(delta, map, w.trees.Values.ToList());
            w.Grid = grid;
            return w;
        }
    }
}
