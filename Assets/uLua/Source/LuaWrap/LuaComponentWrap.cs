using System;
using UnityEngine;
using LuaInterface;
using Object = UnityEngine.Object;

public class LuaComponentWrap
{
	public static void Register(IntPtr L)
	{
		LuaMethod[] regs = new LuaMethod[]
		{
			new LuaMethod("GetLuaComponent", GetLuaComponent),
			new LuaMethod("AddLuaComponent", AddLuaComponent),
			new LuaMethod("Initilize", Initilize),
			new LuaMethod("New", _CreateLuaComponent),
			new LuaMethod("GetClassType", GetClassType),
			new LuaMethod("__eq", Lua_Eq),
		};

		LuaField[] fields = new LuaField[]
		{
			new LuaField("m_luaScript", get_m_luaScript, set_m_luaScript),
			new LuaField("LuaModule", get_LuaModule, null),
		};

		LuaScriptMgr.RegisterLib(L, "LuaComponent", typeof(LuaComponent), regs, fields, typeof(MonoBehaviour));
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int _CreateLuaComponent(IntPtr L)
	{
		LuaDLL.luaL_error(L, "LuaComponent class does not have a constructor function");
		return 0;
	}

	static Type classType = typeof(LuaComponent);

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetClassType(IntPtr L)
	{
		LuaScriptMgr.Push(L, classType);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_m_luaScript(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);
		LuaComponent obj = (LuaComponent)o;

		if (obj == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_luaScript");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_luaScript on a nil value");
			}
		}

		LuaScriptMgr.Push(L, obj.m_luaScript);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int get_LuaModule(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);
		LuaComponent obj = (LuaComponent)o;

		if (obj == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name LuaModule");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index LuaModule on a nil value");
			}
		}

		LuaScriptMgr.Push(L, obj.LuaModule);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int set_m_luaScript(IntPtr L)
	{
		object o = LuaScriptMgr.GetLuaObject(L, 1);
		LuaComponent obj = (LuaComponent)o;

		if (obj == null)
		{
			LuaTypes types = LuaDLL.lua_type(L, 1);

			if (types == LuaTypes.LUA_TTABLE)
			{
				LuaDLL.luaL_error(L, "unknown member name m_luaScript");
			}
			else
			{
				LuaDLL.luaL_error(L, "attempt to index m_luaScript on a nil value");
			}
		}

		obj.m_luaScript = (TextAsset)LuaScriptMgr.GetUnityObject(L, 3, typeof(TextAsset));
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int GetLuaComponent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 1);
		GameObject arg0 = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		LuaInterface.LuaTable o = LuaComponent.GetLuaComponent(arg0);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int AddLuaComponent(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		GameObject arg0 = (GameObject)LuaScriptMgr.GetUnityObject(L, 1, typeof(GameObject));
		TextAsset arg1 = (TextAsset)LuaScriptMgr.GetUnityObject(L, 2, typeof(TextAsset));
		LuaInterface.LuaTable o = LuaComponent.AddLuaComponent(arg0,arg1);
		LuaScriptMgr.Push(L, o);
		return 1;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Initilize(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		LuaComponent obj = (LuaComponent)LuaScriptMgr.GetUnityObjectSelf(L, 1, "LuaComponent");
		TextAsset arg0 = (TextAsset)LuaScriptMgr.GetUnityObject(L, 2, typeof(TextAsset));
		obj.Initilize(arg0);
		return 0;
	}

	[MonoPInvokeCallbackAttribute(typeof(LuaCSFunction))]
	static int Lua_Eq(IntPtr L)
	{
		LuaScriptMgr.CheckArgsCount(L, 2);
		Object arg0 = LuaScriptMgr.GetLuaObject(L, 1) as Object;
		Object arg1 = LuaScriptMgr.GetLuaObject(L, 2) as Object;
		bool o = arg0 == arg1;
		LuaScriptMgr.Push(L, o);
		return 1;
	}
}

