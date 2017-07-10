using System.Collections.Generic;
using System.IO;

namespace BruteforceLib.Generators
{
    class DB2FileNameGenerator : FileNameGenerator
    {
        public DB2FileNameGenerator()
        {
            Index = 0;
            Count = 5;
        }

        public override IEnumerable<string> GetFileNames()
        {
            Index++;

            DB6Reader mid = new DB6Reader(Path.Combine(BruteforceConfig.DB2Folder, "ManifestInterfaceData.db2"));

            foreach (var row in mid)
            {
                yield return string.Format("{0}{1}", row.Value.GetField<string>(0), row.Value.GetField<string>(1));
            }

            Index++;

            DB6Reader ls = new DB6Reader(Path.Combine(BruteforceConfig.DB2Folder, "LightSkybox.db2"));

            foreach (var row in ls)
            {
                if (row.Value.GetField<string>(0) != string.Empty)
                    yield return string.Format("{0}", row.Value.GetField<string>(0));
            }

            Index++;

            DB6Reader maps = new DB6Reader(Path.Combine(BruteforceConfig.DB2Folder, "Map.db2"));

            foreach (var map in maps)
            {
                string folder = map.Value.GetField<string>(0);

                yield return string.Format("World\\Maps\\{0}\\{0}.tex", folder);
                yield return string.Format("World\\Maps\\{0}\\{0}.wdl", folder);
                yield return string.Format("World\\Maps\\{0}\\{0}.wdt", folder);
                yield return string.Format("World\\Maps\\{0}\\{0}_occ.wdt", folder);
                yield return string.Format("World\\Maps\\{0}\\{0}_lgt.wdt", folder);

                for (int i = 0; i < 64; ++i)
                {
                    for (int j = 0; j < 64; ++j)
                    {
                        yield return string.Format("World\\Maps\\{0}\\{0}_{1}_{2}.adt", folder, i, j);
                        yield return string.Format("World\\Maps\\{0}\\{0}_{1}_{2}_lod.adt", folder, i, j);
                        yield return string.Format("World\\Maps\\{0}\\{0}_{1}_{2}_obj0.adt", folder, i, j);
                        yield return string.Format("World\\Maps\\{0}\\{0}_{1}_{2}_obj1.adt", folder, i, j);
                        yield return string.Format("World\\Maps\\{0}\\{0}_{1}_{2}_tex0.adt", folder, i, j);
                        yield return string.Format("World\\Maps\\{0}\\{0}_{1}_{2}_tex1.adt", folder, i, j);

                        yield return string.Format("World\\maptextures\\{0}\\{0}_{1:D2}_{2:D2}.blp", folder, i, j);
                        yield return string.Format("World\\maptextures\\{0}\\{0}_{1:D2}_{2:D2}_n.blp", folder, i, j);

                        yield return string.Format("World\\Minimaps\\{0}\\map{1:D2}_{2:D2}.blp", folder, i, j);
                        yield return string.Format("World\\Minimaps\\{0}\\noLiquid_map{1:D2}_{2:D2}.blp", folder, i, j);
                    }
                }
            }

            Index++;

            //Interface\WorldMap\%s\%s%d.blp WorldMapArea[3], WorldMapArea[3], 1-12
            //Interface\WorldMap\%s\%s%d_%d.blp WorldMapArea[3], WorldMapArea[3], floor, 1-12

            DB6Reader wmas = new DB6Reader(Path.Combine(BruteforceConfig.DB2Folder, "WorldMapArea.db2"));
            DB6Reader wmos = new DB6Reader(Path.Combine(BruteforceConfig.DB2Folder, "WorldMapOverlay.db2"));

            foreach (var wma in wmas)
            {
                var name = wma.Value.GetField<string>(0);

                if ((wma.Value.GetField<int>(10) & 0x200) != 0)
                    name = wmas.GetRow(wma.Value.GetField<int>(9)).GetField<string>(0);

                yield return string.Format(@"Interface\WorldMap\{0}\{0}.zmp", name);

                for (int i = 1; i <= 12; ++i)
                {
                    yield return string.Format(@"Interface\WorldMap\{0}\{0}{1}.blp", name, i);

                    for (int j = 0; j < 30; ++j)
                        yield return string.Format(@"Interface\WorldMap\{0}\{0}{1}_{2}.blp", name, j, i);
                }

                var id = wma.Key;

                foreach (var wmo in wmos)
                {
                    if (wmo.Value.GetField<int>(3) == id)
                    {
                        for (int i = 1; i <= 20; ++i)
                        {
                            yield return string.Format(@"Interface\WorldMap\{0}\{1}{2}.blp", name, wmo.Value.GetField<string>(0), i);
                        }
                    }
                }
            }

            Index++;

            DB6Reader at = new DB6Reader(Path.Combine(BruteforceConfig.DB2Folder, "AreaTable.db2"));

            foreach (var a in at)
            {
                var name = a.Value.GetField<string>(1);

                for (int i = 1; i <= 12; ++i)
                {
                    yield return string.Format(@"Interface\WorldMap\{0}\{0}{1}.blp", name, i);

                    for (int j = 0; j < 30; ++j)
                        yield return string.Format(@"Interface\WorldMap\{0}\{0}{1}_{2}.blp", name, j, i);
                }
            }

            //Interface\WorldMap\MicroDungeon\%s\%s\%s%d.blp WorldMapArea[3], ?, ?, 1-12
            //Interface\WorldMap\MicroDungeon\%s\%s\%s%d_%d.blp WorldMapArea[3], ?, ?, floor, 1-12

            foreach (var wma in wmas)
            {
                var name = wma.Value.GetField<string>(0);

                var areaid = wma.Value.GetField<int>(6);

                //if ((wma.Value.GetField<int>(10) & 0x200) != 0)
                //    areaid = wma.Value.GetField<int>(6);

                foreach (var a in at)
                {
                    if (a.Value.GetField<int>(5) == areaid)
                    {
                        for (int i = 1; i <= 12; ++i)
                        {
                            var md = a.Value.GetField<string>(1);

                            yield return string.Format(@"Interface\WorldMap\MicroDungeon\{0}\{1}\{1}{2}.blp", name, md, i);

                            for (int j = 0; j < 39; ++j)
                                yield return string.Format(@"Interface\WorldMap\MicroDungeon\{0}\{1}\{1}{2}_{3}.blp", name, md, j, i);
                        }
                    }
                }
            }
        }
    }
}
