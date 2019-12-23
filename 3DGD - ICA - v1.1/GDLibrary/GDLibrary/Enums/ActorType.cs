/*
Function: 		Used by Actor to help us distunguish one type of actor from another when we perform CD/CR or when we want to enable/disable certain game entities
                e.g. hide all the pickups.
Author: 		NMCG
Version:		1.0
Last Updated:
Bugs:			None
Fixes:			None
*/

namespace GDLibrary
{
    public enum ActorType : sbyte
    {
        //non-collidable
        Player,
        Camera,
        Helper,
        Billboard,
        Decorator,

        //collidable
        CollidableGround,
        CollidableProp,
        CollidablePickup,
        CollidableArchitecture,
        CollidableZone,
        CollidableCamera,
        CollidablePlayer,
        //ui
        UIMouse,
        UIButton,
        UIText,
        UITexture,
    }
}














