using EscapeFromTheWoods.Database;
using System;
using System.Collections.Generic;
using System.Text;

namespace EscapeFromTheWoods
{
    public static class WoodBuilder
    {        
        public static Wood GetWood(int size,Map map,string path,MongoDBRepository repo)
        {
            Random r = new Random(100);
            Dictionary<int, Tree> trees = new Dictionary<int, Tree>();
            int n = 0;
            while(n<size)
            {
                Tree t = new Tree(IDgenerator.GetTreeID(),r.Next(map.xmin,map.xmax),r.Next(map.ymin,map.ymax));
                if (!trees.ContainsKey(t.treeID)) { trees.Add(t.treeID, t); n++; }
            }
            Wood w = new Wood(IDgenerator.GetWoodID(),trees,map,path,repo);
            return w;
        }
    }
}
