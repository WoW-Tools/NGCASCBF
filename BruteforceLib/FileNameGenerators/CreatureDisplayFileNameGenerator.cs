﻿using System.Collections.Generic;

namespace BruteforceLib.Generators
{
    class CreatureDisplayFileNameGenerator : FileNameGenerator
    {
        public CreatureDisplayFileNameGenerator()
        {
            Count = 1000000;
        }

        public override IEnumerable<string> GetFileNames()
        {
            for (int i = 0; i < 1000000; i++)
            {
                Index = i;

                yield return string.Format("Textures\\BakedNpcTextures\\CreatureDisplayExtra-{0}.blp", i);
                yield return string.Format("Textures\\BakedNpcTextures\\CreatureDisplayExtra-{0}_HD.blp", i);
            }
        }
    }
}
