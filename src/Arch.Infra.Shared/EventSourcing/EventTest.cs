namespace Arch.Infra.Shared.EventSourcing
{
    public class EventTest
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public decimal Dec { get; set; }
    }

    public class OtherClass: SourceFluent<EventTest>
    {
        public override void Configuration(SourceFluent<EventTest> builder)
        {
            builder.Ignore(_ => _.Address, _ => _.Email)
                   .Ignore(_ => _.Dec);
        }
    }
}
