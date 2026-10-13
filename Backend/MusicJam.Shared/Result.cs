using Shared.Errors;

namespace Shared;

public class Result<TValue, TFault>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private TValue Value { get; }
    private TFault Fault { get; }

    private Result(bool isSuccess, TValue value, TFault fault)
    {
        IsSuccess = isSuccess;
        Value = value;
        Fault = fault;
    }

    public static Result<TValue, TFault> Success(TValue value) => new(true, value, default!);

    public static Result<TValue, TFault> Failure(TFault fault) => new(false, default!, fault);

    public static implicit operator Result<TValue, TFault>(TValue value) => Success(value);

    public static implicit operator Result<TValue, TFault>(TFault fault) => Failure(fault);

    public TValue GetValueOrDefault() => IsSuccess ? Value : default!;

    public TValue GetValueOrThrow() => IsSuccess ? Value : throw new ResultException<TFault>(Fault);

    public bool TryGetFault(out TFault fault, out TValue value)
    {
        if (IsSuccess)
        {
            fault = default!;
            value = Value;

            return false;
        }

        fault = Fault;
        value = default!;

        return true;
    }

    public bool TryGetValue(out TValue value, out TFault fault)
    {
        if (IsSuccess)
        {
            fault = default!;
            value = Value;

            return true;
        }

        fault = Fault;
        value = default!;

        return false;
    }

    public void EnsureSuccess()
    {
        if (IsSuccess)
        {
            return;
        }

        throw new ResultException<TFault>(Fault);
    }
}

public class Result<TFault>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public TFault Fault { get; }

    private Result(bool isSuccess, TFault fault)
    {
        IsSuccess = isSuccess;
        Fault = fault;
    }

    public static Result<TFault> Success() => new(true, default!);

    public static Result<TFault> Failure(TFault fault) => new(false, fault);

    public static implicit operator Result<TFault>(TFault fault) => Failure(fault);

    public bool TryGetFault(out TFault fault)
    {
        if (IsSuccess)
        {
            fault = default!;
            return false;
        }

        fault = Fault;
        return false;
    }

    public void EnsureSuccess()
    {
        if (IsSuccess)
        {
            return;
        }

        throw new ResultException<TFault>(Fault);
    }
}
