using System.Drawing;
using UnityEngine.UI;

namespace Infissy.Properties
{


    public static class GameProperties{

        public static class GameInitializationValues{
            public const int CardDrawNumber = 1;
            public const int InitialPlayerGold = 500;
            public const int InitialPlayerResources = 500;
            public const int InitialPlayerPopulation = 1500;
        }

        public static class CardEventData
        {
            public enum CardEventType
            {
                cardDestroyed,
                cardInteracted,
                cardPlayed
                //To eventually continue
            }
        }

        public enum ClickPriority
        {
            notYetDefined,
            localFirstClick,
            enemyFirstClick
        }

        public enum GamePhase{

            DrawPhase,
            PlayPhase,
            MovePhase,
            AttackPhase

        }
     

    }

    public static class DisplayProperties
    {
      
        public enum CardStatus
        {
            Selected,
            Normal
        }

        public enum CardDisplayType
        {
            HandCard,
            UnitCard,
            StructureCard,

        }


        public struct ResourcesDisplayText
        {
            public Text Population;
            public Text Gold;
            public Text Resources;
            public Text Progress;
        }


    }
    public static class NetworkProperties
    {
        public enum MessageType
        {
            Move,
            GameStatus,
            PlayerStatus
        }

        public static class MoveMessageProperties
        {
            public enum MoveMessageData
            {
                MessageType,
                IDCardPlayed,
                IDTargetCard
            }

            public enum MoveMessageType
            {
                Active = 3,
                Targetless = 2
            }
        }


    }
    public static class CardProperties
    {
        public enum CardType
        {
            Attack,
            Structure,
            PassiveSpell,
            ActiveSpell,
            Trap,
            Progress
        }

        public enum CardReferenceCity
        {
            Belrik,
            Zrata,
            Venous,
            Ereco,
            Other
        }

        public enum CardRarity
        {
            Copper,
            Steel,
            Brass,
            Silver,
            Gold
        }

        /// <summary>
        /// Definisce il target dell'effetto, si suddivide in nemico e alleato come : avversario e se stesso
        /// 
        ///     Gold Materials Population sono le 3 risorse
        ///     Structure Unit sono costruzioni e unità
        /// </summary>
        public enum CardEffectTarget
        {

            AllyGold,
            AllyResources,
            AllyPopulation,
            AllyStructure,
            AllyUnit,
            EnemyGold,
            EnemyResources,
            EnemyPopulation,
            EnemyStructure,
            EnemyUnit,
            Healable,
            Targetable,
            None

        }

        /// <summary>
        /// Descrive il tipo di effetto delle carte. 
        /// Si suddivide in
        /// 
        ///     Value/Percentual per modificare un determinato valore, per un incremento 
        ///
        ///     Complex Value, che implica operazioni matematiche o ulteriori dati presi da altre classi. (IE L'aumento percentuale delle risorse grazie a una costruzione)
        ///  
        ///</summary>
        public enum CardEffectType
        {
            ValueIncrement,
            PercentualIncrement,
            ValueBased,
            CardDraw,
            EndGame,
            Healable,
            Destructible,
            Targetable,
            Effect
        }







    }

}