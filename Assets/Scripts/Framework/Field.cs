using System;
using UnityEngine;
using UnityEngine.UI;
using Infissy.Framework;



//Gestisce le carte a livello più basso, si occupa delle loro posizioni

namespace Infissy.Framework{



    public class Field
    {

 
        
    
  



    private Player player;
    private Player remotePlayer;

    public Player Player{ get {return player;}  }
    public Player RemotePlayer{ get {return remotePlayer;}  }
    ///

    public void AffectPlayer (int effectValue, CardEffectTarget affectTarget){

    }
    //AffectCard
    
    public static Field Initialize (Player player, RemotePlayer remotePlayer){
        
        Field field = new Field();

        field.player = player;
        field.remotePlayer = remotePlayer;

        return field;
    }

    ///
    }
}
