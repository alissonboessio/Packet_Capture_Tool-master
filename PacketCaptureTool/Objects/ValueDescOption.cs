
namespace PacketCaptureTool.Objects
{
    class ValueDescOption : PropChange
    {
        public ValueDescOption(int value, string description)
        {
            this.value = value;
            this.description = description;
        }

        public bool check
        {
            get => Get<bool>();
            set
            {
                Set(!check);
                onCheck?.Invoke(this);
            }
        }
        public int value { get => Get<int>(); set => Set(value); }
        public string description { get => Get<string>(); set => Set(value); }
        public Action<ValueDescOption> onCheck { get => Get<Action<ValueDescOption>>(); set => Set(value); }

    }
}
