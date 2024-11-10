using System.ComponentModel;
using SkillQuest.Shared.Game.ECS;
using IComponent = SkillQuest.Shared.Game.ECS.IComponent;

namespace SkillQuest.Shared.Game.Addons.Shared.SkillQuest;

public class AddonSkillQuestSH : Addon{
    public AddonSkillQuestSH(){
        Mounted += OnMounted;
        Unmounted += OnUnmounted;
    }

    void Setup(IThing thing){
        thing.ConnectComponent += (t, c) => {
            Console.WriteLine("{0} ConnectComponent: {0} -> {1}", t.Uri, ( c as Component< IComponent > )?.Name);
        };

        thing.DisconnectComponent += (t, c) => {
            Console.WriteLine("{0} DisconnectComponent: {0} -x> {1}", t.Uri, ( c as Component< IComponent > )?.Name);
        };

        thing.Parented += (parent, child) => { Console.WriteLine("{0} Parent: {0} -> {1}", parent.Uri, child.Uri); };

        thing.Unparented += (parent, child) => {
            Console.WriteLine("{0} Unparent: {0} -x> {1}", parent.Uri, child.Uri);
        };

        thing.AddChild += (parent, child) => {
            Console.WriteLine(" {1} Add Child: {0} -> {1}", parent.Uri, child.Uri);
        };

        thing.RemoveChild += (parent, child) => {
            Console.WriteLine(" {1} Remove Child: {0} -x> {1}", parent.Uri, child.Uri);
        };
    }

    public void Setup(IComponent component){
        component.ConnectThing += (thing, component) => {
            Console.WriteLine("{1} ConnectThing: {1} -> {0}", thing.Uri, ( component as Component< IComponent > )?.Name );
        };

        component.DisconnectThing += (thing, component) => {
            Console.WriteLine("{1} DisconnectThing: {1} -x> {0}", thing.Uri, ( component as Component< IComponent > )?.Name);
        };
    }

    void OnMounted(Addon addon, IApplication? application){
        var stuff = new Stuff();

        var thingA = new Thing(new Uri("thing://skill.quest/a"), stuff);
        Setup(thingA);
        var thingB = new Thing(new Uri("thing://skill.quest/b"), stuff);
        Setup(thingB);
        var thingC = new Thing(new Uri("thing://skill.quest/c"), stuff);
        Setup(thingC);
        var thingD = new Thing(new Uri("thing://skill.quest/d"), stuff);
        Setup(thingD);

        var componentA = new Component<IComponent>();
        componentA.Name = "A";
        Setup(componentA);
        var componentB = new Component<IComponent>();
        componentB.Name = "B";
        Setup(componentB);
        var componentC = new Component<IComponent>();
        componentC.Name = "C";
        Setup(componentC);

        thingA[thingB.Uri] = thingB;

        Console.WriteLine();
        
        thingB[thingC] = true;
        Console.WriteLine();
        thingD.Parent = thingC;
        Console.WriteLine();

        thingA.Component(componentA);
        Console.WriteLine( "thingA.componentA == componentA {0}", ( thingA[ componentA.GetType() ] as Component< IComponent > ).Name );

        Console.WriteLine();
        
        thingA.Component(componentB);
        Console.WriteLine(  "thingA.componentB == componentB {0}", ( thingA[ componentB.GetType() ] as Component< IComponent > )?.Name );
        Console.WriteLine(  "thingB.componentB == componentB {0}", ( thingB[ componentB.GetType() ] as Component< IComponent > )?.Name );
        Console.WriteLine();

        thingB.Component(componentB);

        Console.WriteLine(  "thingA.componentB == componentB {0}", ( thingA[ componentB.GetType() ] as Component< IComponent > )?.Name );
        Console.WriteLine(  "thingB.componentB == componentB {0}", ( thingB[ componentB.GetType() ] as Component< IComponent > )?.Name );
        Console.WriteLine();

        thingC.Component(componentC);
        Console.WriteLine(  "thingC.componentC == componentC {0}", ( thingC[ componentC.GetType() ] as Component<IComponent >)?.Name );
        Console.WriteLine();
        
        thingC.Component(null, componentC.GetType());
        Console.WriteLine(  "thingC.componentC == null", thingC.Components.ContainsKey(componentC.GetType()) );
        Console.WriteLine();

    }

    void OnUnmounted(Addon addon, IApplication? application){ }
}
