using BS;

namespace StakeDriver
{
    // This create an item module that can be referenced in the item JSON
    public class ItemModuleStakeDriver : ItemModule
    {

        public float expForceMultiplier = 10f;
        public float explosionRadius = 5f;
        public float switchSpeed = 5f;
        public float chargeTime = 4f;
        public override void OnItemLoaded(Item item)
        {
            base.OnItemLoaded(item);
            item.gameObject.AddComponent<ItemStakeDriver>();
        }
    }
}
