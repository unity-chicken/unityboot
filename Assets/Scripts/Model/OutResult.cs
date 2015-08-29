using UnityEngine;
using System.Collections;

public class OutResult<T> {
    public T value;
}

public class BoolResult : OutResult<bool> {
    public bool IsSuccess() { return value == true; }
    public bool IsFailed() { return value == false; }
    public void SetSuccess(bool success) { value = success; }
}

public class NetResult : BoolResult {
}

public class NetResult<T> : NetResult {
    public T result;
}
