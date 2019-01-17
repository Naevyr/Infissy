namespace InfissyProperties {

     public static class CardProperties {
        public static enum CardType {
            Attack, Structure, PassiveSpell, ActiveSpell, Trap
        }

        public static enum CardReferenceCity {
            Belrik, Zrata, Venous, Ereco, Other
        }

        public static enum CardRarity {
            Copper, Steel, Brass, Silver, Gold
        }

        /// <summary>
        /// Definisce il target dell'effetto, si suddivide in nemico e alleato come : avversario e se stesso
        /// 
        ///     Gold Materials Population sono le 3 risorse
        ///     Structure Unit sono costruzioni e unità
        /// </summary>
        public static class CardEffectTarget {
            static public enum Enemy {
                EnemyGold, EnemyMaterials, EnemyPopulation, EnemyStructure, EnemyUnit
            }
            static public enum Ally {
                AllyGold, AllyMaterials, AllyPopulation, AllyStructure, AllyUnit
            }

       }



        /// <summary>
        /// Descrive il tipo di effetto delle carte. 
        /// Si suddivide in
        /// 
        ///     Fixed Value, che implica un valore finito da aggiungere o togliere al target dell'effetto. (IE L'attacco di una truppa verso un'altra)
        ///
        ///     Complex Value, che implica operazioni matematiche o ulteriori dati presi da altre classi. (IE L'aumento percentuale delle risorse grazie a una costruzione)
        ///  
        /// /// </summary>
        public static class CardEffectType{

           public static enum FixedValue{
               ValueDecrease, ValueIncrease
           }

           public static enum ComplexValue
           {
               Percentual, ValueBased
           }

           public static enum StatusEffect {

               Healable, Destructible, Targetable
           }
       }
            
             

        
        public struct CardEffect{
            
            public CardTargetEffect TargetEffect;
            public int EffectValue;
            public CardEffectType EffectType;



        }



        
    }


}