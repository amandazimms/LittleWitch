using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorFlag : MonoBehaviour
{
    ///todo reword this so it's clearer
    /// 
    ///utility script used by ColorVarianceSprites (and others?)
    ///Example: mouse has two colors, brown and pink. Various body parts need one color or the other.
    ///Attach this script to every body part. Check bool (true) for flag A on all brown parts, and flag B for pink.
    ///ColorVarianceSprites script is on Mouse's main parent object. In that, check bool true for ColorFlag.
    ///It will then assign all GOs with flag A checked to one CVS, and all the Bs to another. 

    public bool flagA;
    public bool flagB;
    public bool flagC;
    public bool flagD;
    public bool flagE;
}
