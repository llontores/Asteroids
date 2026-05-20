using System;
using MVVM;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

public sealed class MonoViewBinder : MonoBehaviour
{
    private enum BindingMode
    {
        FromInstance = 0,
        FromResolve = 1,
        FromResolveId = 2
    }

    [SerializeField]
    private BindingMode viewBinding;

    [ShowIf(nameof(viewBinding), BindingMode.FromInstance)]
    [SerializeField]
    private Object view;

#if UNITY_EDITOR
    [ShowIf("@this.viewBinding == BindingMode.FromResolve || this.viewBinding == BindingMode.FromResolveId")]
    [SerializeField]
    private MonoScript viewType;
#endif

    [SerializeField, HideInInspector]
    private string viewTypeName;

    [ShowIf(nameof(viewBinding), BindingMode.FromResolveId)]
    [SerializeField]
    private string viewId;

    [Space(8)]
    [SerializeField]
    private BindingMode viewModelBinding;

    [ShowIf(nameof(viewModelBinding), BindingMode.FromInstance)]
    [SerializeField]
    private Object viewModel;

#if UNITY_EDITOR
    [ShowIf("@this.viewModelBinding == BindingMode.FromResolve || this.viewModelBinding == BindingMode.FromResolveId")]
    [SerializeField]
    private MonoScript viewModelType;
#endif

    [SerializeField, HideInInspector]
    private string viewModelTypeName;

    [ShowIf(nameof(viewModelBinding), BindingMode.FromResolveId)]
    [SerializeField]
    private string viewModelId;

    [Inject]
    private DiContainer diContainer;

    private IBinder _binder;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (viewType != null)
        {
            var type = viewType.GetClass();
            if (type != null)
                viewTypeName = type.AssemblyQualifiedName;
        }

        if (viewModelType != null)
        {
            var type = viewModelType.GetClass();
            if (type != null)
                viewModelTypeName = type.AssemblyQualifiedName;
        }
    }
#endif

    private void Start()
    {
        _binder = CreateBinder();
        _binder.Bind();
    }

    private void OnEnable()
    {
        _binder?.Bind();
    }

    private void OnDisable()
    {
        _binder?.Unbind();
    }

    private IBinder CreateBinder()
    {
        Type viewResolvedType = !string.IsNullOrEmpty(viewTypeName) ? Type.GetType(viewTypeName) : null;
        Type viewModelResolvedType = !string.IsNullOrEmpty(viewModelTypeName) ? Type.GetType(viewModelTypeName) : null;

        object view = viewBinding switch
        {
            BindingMode.FromInstance => this.view,
            BindingMode.FromResolve => diContainer.Resolve(viewResolvedType),
            BindingMode.FromResolveId => diContainer.ResolveId(viewResolvedType, viewId),
            _ => throw new Exception($"Binding type of view {viewBinding} is not found!")
        };

        object model = viewModelBinding switch
        {
            BindingMode.FromInstance => this.viewModel,
            BindingMode.FromResolve => diContainer.Resolve(viewModelResolvedType),
            BindingMode.FromResolveId => diContainer.ResolveId(viewModelResolvedType, viewModelId),
            _ => throw new Exception($"Binding type of viewModel {viewModelBinding} is not found!")
        };

        return BinderFactory.CreateComposite(view, model);
    }
}