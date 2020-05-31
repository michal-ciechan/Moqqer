using System;

namespace MoqqerNamespace.Tests.TestClasses
{
    public interface IInterfaceWithOutAndRefParamMethods
    {
        bool MethodWithRefAndOut(out bool outParam, ref bool refParam);
        bool MethodWithRef(ref bool refParam);
        bool MethodWithOut(out bool refParam);
        
        bool MethodWithRefAndOut(out Root outParam, ref Root refParam);
        bool MethodWithRef(ref Root refParam);
        bool MethodWithOut(out Root outParam);
        
        bool MethodWithRefAndOut(out IDepencyA outParam, ref IDepencyA refParam);
        bool MethodWithRef(ref IDepencyA refParam);
        bool MethodWithOut(out IDepencyA outParam);
        
        IDisposable MethodWithRefAndOutAndInterfaceReturn(out bool outParam, ref bool refParam);
        IDisposable MethodWithOutAndInterfaceReturn(out bool outParam);
        IDisposable MethodWithRefAndInterfaceReturn(ref bool outParam);
        
        IDisposable MethodWithRefAndOutAndInterfaceReturn(out Root outParam, ref Root refParam);
        IDisposable MethodWithOutAndInterfaceReturn(out Root outParam);
        IDisposable MethodWithRefAndInterfaceReturn(ref Root outParam);
        
        IDisposable MethodWithRefAndOutAndInterfaceReturn(out IDepencyA outParam, ref IDepencyA refParam);
        IDisposable MethodWithOutAndInterfaceReturn(out IDepencyA outParam);
        IDisposable MethodWithRefAndInterfaceReturn(ref IDepencyA outParam);
    }
}