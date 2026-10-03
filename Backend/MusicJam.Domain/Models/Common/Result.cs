using MusicJam.Domain.Models.Errors;

namespace MusicJam.Domain.Models.Common;

public class Result<TValue, TFault>
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public TValue Value { get; }
    public TFault Fault { get; }

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

    public void EnsureSuccess()
    {
        if (IsSuccess)
        {
            return;
        }

        throw new ResultException<TFault>(Fault);
    }
}