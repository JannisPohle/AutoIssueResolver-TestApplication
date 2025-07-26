public static Result<TResult> Success(TResult payload){
    return new Result<TResult>{
      Payload = payload,
      Exception = null,
    }; 
  }