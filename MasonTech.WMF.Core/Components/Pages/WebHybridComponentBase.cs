using MasonTech.WMF.Core.Components.Objects;
using MasonTech.WMF.Test.HybridBridge;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Reflection;
using System.Text.Json;

namespace MasonTech.WMF.Core.Components.Pages
{
    public abstract class WebHybridComponentBase<T> : ComponentBase where T : IHybridBridge, new()
    {
        public T ComponentBridge { get; set; }

        public WebHybridComponentBase()
        {
            ComponentBridge = new T();
        }

        [Inject]
        public IJSRuntime? CurrentJSRuntime { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await SendDotNetInstanceToJS();
            }
        }

        public async Task SendDotNetInstanceToJS()
        {
            var dotNetObjRef = DotNetObjectReference.Create(this);
            if (CurrentJSRuntime != null)
            {
                await CurrentJSRuntime.InvokeVoidAsync(GlobalFunctionNames.RegisterDotNetObjRef, dotNetObjRef);
            }
        }


        [JSInvokable]
        public async Task<string> ActiveNativeMethod(ActiveNativeMethodParams requestParams)
        {
            MethodInfo? methodInfo = ComponentBridge.GetType().GetMethod(requestParams.Name);
            if (methodInfo == null)
            {
                return "Cannot get method by name: " + requestParams.Name;
            }
            else
            {
                List<object?> methodObjects = new List<object?>();
                ParameterInfo[] parameterInfos = methodInfo.GetParameters();
                foreach (ParameterInfo parameterInfo in parameterInfos)
                {
                    Type? realType = parameterInfo.ParameterType;
                    if (realType != null)
                    {
                        bool isClass = realType.GetType().GetTypeInfo().IsClass;
                        if (isClass)
                        {
                            object? objectValue = JsonSerializer.Deserialize(requestParams.Data, realType);
                            methodObjects.Add(objectValue);
                        }
                        else
                        {
                            object? objectValue = Convert.ChangeType(requestParams.Data, realType);
                            methodObjects.Add(objectValue);
                        }
                    }
                }

                object? result = methodInfo.Invoke(ComponentBridge, methodObjects.ToArray());
                if (result == null)
                {
                    return string.Empty;
                }
                else
                {
                    if (result.GetType().BaseType?.FullName == "System.Threading.Tasks.Task")
                    {
                        await (Task)result;
                        var taskResult = result.GetType().GetProperty("Result")?.GetValue(result, null);
                        return JsonSerializer.Serialize(taskResult);
                    }
                    else
                    {
                        return JsonSerializer.Serialize(result);
                    }
                }
            }
        }
    }
}
