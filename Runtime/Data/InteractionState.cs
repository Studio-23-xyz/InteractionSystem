namespace Studio23.SS2.InteractionSystem.Data
{
    public enum InteractionState{
        Inactive,//interaction hasn't started yet.
        Active,//runing interaction logic
        Paused,// running a sub-interaction or some other case when we don't want the interaction to run logic
    }
}