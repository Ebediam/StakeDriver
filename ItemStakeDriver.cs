using BS;
using UnityEngine;

namespace StakeDriver
{

    public class ItemStakeDriver : MonoBehaviour
    {
       
        protected Item item;

        public ItemModuleStakeDriver module;
        public Transform spear;
        public bool isExtended = true;
        public bool isChanging = false;
        public float position = 0.1275f;
        public float count = 0;
        public bool isExplosionReady = false;


        public AudioSource trickSFX;
        public AudioSource explosionSFX;
        public AudioSource explosionReadySFX;
        public AudioSource defuseSFX;

        public ParticleSystem sparksVFX;
        public ParticleSystem explosionVFX;

        protected void Awake()
        {

            item = this.GetComponent<Item>();
            item.OnHeldActionEvent += OnHeldAction;
            item.OnCollisionEvent += OnStakeCollision;

            module = item.data.GetModule<ItemModuleStakeDriver>();
            trickSFX = item.transform.Find("TrickSFX").GetComponent<AudioSource>();
            explosionSFX = item.transform.Find("ExplosionSFX").GetComponent<AudioSource>();
            explosionReadySFX = item.transform.Find("ExplosionReadySFX").GetComponent<AudioSource>();
            defuseSFX = item.transform.Find("DefuseSFX").GetComponent<AudioSource>();

            sparksVFX = item.transform.Find("SparksVFX").GetComponent<ParticleSystem>();
            explosionVFX = item.transform.Find("ExplosionVFX").GetComponent<ParticleSystem>();


            spear = item.transform.Find("Spear");



        }


        public void OnStakeCollision(ref CollisionStruct collisionInstance)
        {
            if (!isExtended && !isExplosionReady)
            {
                CancelInvoke("ReadyExplosion");
                Invoke("ReadyExplosion", module.chargeTime);
                defuseSFX.Play();
            }

        }
       
        public void OnHeldAction(Interactor interactor, Handle handle, Interactable.Action action)
        {

            if (action == Interactable.Action.UseStart && !isChanging)
            {
                isChanging = true;
            }
        }

        void FixedUpdate()
        {
            if (isChanging)
            {
                ChangeWeapon();
            }

            

        }

      
        public void ChangeWeapon()
        {
            if (isExtended)
            {
                spear.position = spear.position - spear.forward.normalized * (position / module.switchSpeed);

                count++;
                if(count >= module.switchSpeed)
                {

                    trickSFX.Play();
                    sparksVFX.Play();
                    count = 0f;
                    isExtended = false;
                    isChanging = false;
                    Invoke("ReadyExplosion", module.chargeTime);


                }
            }
            else
            {
                spear.position = spear.position + spear.forward.normalized * (position / module.switchSpeed);
                count++;
                if (count >= module.switchSpeed)
                {
                    
                    trickSFX.Play();
                    sparksVFX.Play();
                    count = 0f;
                    isExtended = true;
                    isChanging = false;
                    CancelInvoke("ReadyExplosion");
                    if (isExplosionReady)
                    {
                        explosionSFX.Play();
                        explosionVFX.Play();
                        isExplosionReady = false;
                        Explosion(module.explosionRadius);
                    }

                }
            }
        }

        public void ReadyExplosion()
        {
            isExplosionReady = true;
            explosionReadySFX.Play();
            CancelInvoke("ReadyExplosion");
        }

        void Explosion(float rad)
        {
            
            foreach (Creature npc in Creature.list)
            {
                if (npc != Creature.player)
                {
                    float distNPC = Vector3.Distance(npc.body.headBone.transform.position, item.transform.position);
                    if (distNPC < rad)
                    {
                        float expReductor = (rad - distNPC) / rad;
                        expReductor = expReductor * 0.5f + 0.5f;
                        Vector3 expForceDirection = npc.body.headBone.transform.position - item.transform.position;
                        expForceDirection.Normalize();

                        if (npc.state != Creature.State.Dead)
                        {
                            npc.ragdoll.SetState(BS.Ragdoll.State.Fallen);
                        }
                        foreach (RagdollPart ragdollPart in npc.ragdoll.parts)
                        {
                            ragdollPart.rb.AddForce(expForceDirection * module.expForceMultiplier * expReductor, ForceMode.Impulse);

                        }
                        npc.health.currentHealth -= 200f * expReductor;

                    }

                }

            }
        }
    }
}