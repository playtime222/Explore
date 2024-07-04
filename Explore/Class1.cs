using Metalama.Extensions.Architecture.Aspects;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Fabrics;
using Metalama.Patterns.Contracts;
using System.ComponentModel;

namespace Explore
{
    [DerivedTypesMustRespectNamingConvention("*Careless")]
    public interface IDontCare
    {
        string NorAboutThisOne();
    }

    public class Class1Careless : IDontCare
    {
        public string NorAboutThisOne()
        {
            throw new NotImplementedException();
        }
    }


    public interface IContracts
    {
        void DoSomething([Required] string text);
        void DoSomething([Range(2, 5)] int value);
    }

    public class Contracted : IContracts
    {
        private string? _StopWarnings;

        public void DoSomething(string text)
        {
            _StopWarnings = text;
        }

        public void DoSomething(int value)
        {
        }
    }

    namespace Deeper
    {
        public class Uncontracted
        {
            private string? _StopWarnings;

            public void DoSomething(string text)
            {
                _StopWarnings = text;
            }
        }

    }
}

internal partial class ProjectAspects : ProjectFabric
{
    public override void AmendProject(IProjectAmender amender)
    {
        amender.AddRequiredAspect();
    }
}

[CompileTime]
public static class FabricExtensions
{
    public static void AddRequiredAspect(this IProjectAmender amender)
    {
        var types = amender.Outbound.SelectMany(
            x => x.GlobalNamespace
            .DescendantsAndSelf()
            .SelectMany(n => n.Types.Where(x => x.Namespace.Name == nameof(Explore.Deeper))));

        var mb = types.SelectMany(t => t.Methods.Cast<IMethodBase>().Concat(t.Constructors).Concat(t.Properties.Select(p => p.SetMethod).Where(g => g != null)))
            .Where(m => m.Accessibility != Accessibility.Private);

        mb.SelectMany(m => m.Parameters)
            .Where(p => p.RefKind == RefKind.None && p.Type.IsReferenceType == true && p.Type.IsNullable != true)
            .AddAspectIfEligible<RequiredAttribute>();
    }
}

[Inheritable]
internal class NotifyPropertyChangedAttribute : TypeAspect
{
    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        builder.Advice.ImplementInterface(builder.Target, typeof(INotifyPropertyChanged), OverrideStrategy.Ignore);

        foreach (var property in builder.Target.Properties.Where(p => !p.IsAbstract && p.Writeability == Writeability.All))
        {
            builder.Advice.OverrideAccessors(property, null, nameof(this.OverridePropertySetter));
        }
    }

    [InterfaceMember]
    public event PropertyChangedEventHandler PropertyChanged;

    [Introduce(WhenExists = OverrideStrategy.Ignore)]
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(meta.This, new PropertyChangedEventArgs(name));

    [Template]
    private dynamic OverridePropertySetter(dynamic value)
    {
        if (value != meta.Target.Property.Value)
        {
            meta.Proceed();
            OnPropertyChanged(meta.Target.Property.Name);
        }

        return value;
    }
}

[NotifyPropertyChanged]
public class AnyOldViewModel
{ 
    public string Name { get; set; }
}