using UnityEngine;

namespace Potato.Utils
{
    public static class LayerMaskHelper
    {
        static readonly LayerMask[] cache = new LayerMask[32];

        // useful for performing Physics.Overlap checks using own layer, without duplicating the collision matrix in scripts
        public static LayerMask GetCollisionMaskForLayer(int layer)
        {
            if (cache[layer].value != 0)
                return cache[layer];

            int mask = 0;
            for (int i = 0; i < 32; i++)
                if (!Physics.GetIgnoreLayerCollision(layer, i))
                    mask |= 1 << i;

            cache[layer] = mask;
            return mask;
        }
    }
}