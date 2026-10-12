using FluentValidation.Results;
using Microsoft.AspNetCore.Identity;

namespace ZenBlog.Application.Base;

public class BaseResult<T>
{
    //iþlem baþarýlýysa dönecek veri
    //hata durumunda veri olmayabýlýr null olabilir
    //immutuble yapýyoruz init olunca
    public T? Data { get; init; }
    public IReadOnlyList<Error> Errors { get; init; } = new List<Error>();//boþ liste yapýyoruz eger bossa yaný count 0 ise dogrudur demek ýcýn 
    public bool IsSuccessful => Errors.Count == 0;


    //get iþlemelri dolu dýnuyorya oonlar ýcýn
    public static BaseResult<T> Success(T data)
    {
        return new BaseResult<T>
        {
            Data = data
        };
    }


    //post put delete bos donuyor onun ýcjn
    public static BaseResult<T> Success()
    {
        return new BaseResult<T>();
    }
    
    public static BaseResult<T> Fail(IEnumerable<ValidationFailure> validationFailures)
    {
        var errors = (from error in validationFailures
                      select new Error
                      {
                          ErrorMessage = error.ErrorMessage,
                          PropertyName = error.PropertyName
                      }).ToList();

        return new BaseResult<T>
        {
            Errors = errors
        };


    }



    public static BaseResult<T> Fail(IEnumerable<IdentityError> identityErrors)
    {
        var errors = (from error in identityErrors
                      select new Error
                      {
                          ErrorMessage = error.Description,
                          PropertyName = error.Code
                      }).ToList();

        return new BaseResult<T>
        {
            Errors = errors
        };
    }

    public static BaseResult<T> Fail(string message)
    {
        return new BaseResult<T>
        {
            Errors = new List<Error>
            {
                new Error
                {
                    ErrorMessage=message,
                    PropertyName=string.Empty
                }
            }


        };
    }






}


public class Error
{

    public string PropertyName { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;


}
