namespace Infissy.Properties
{
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