# 说明

用于错误的数据类型，包含错误码和错误信息。


# 使用

```c sharp
XTC.Types.Error err = new XTC.Types.Error(3, "my error");
Debug.Log(err.IsOK);
Debug.Log(err.code);
Debug.Log(err.message);

// 无错误
err = XTC.Types.Error.OK;
// 空值错误
err = XTC.Types.Error.NewNullErr("");
if(err.code == XTC.Types.Error.NULL) {}
// 参数错误
err = XTC.Types.Error.NewParamErr("");
if(err.code == XTC.Types.Error.PARAM) {}
// 访问错误
err = XTC.Types.Error.NewAccessErr("");
if(err.code == XTC.Types.Error.ACCESS) {}
// 异常
err = XTC.Types.Error.NewException(ex);
if(err.code == XTC.Types.Error.EXCEPTION) {}
```

