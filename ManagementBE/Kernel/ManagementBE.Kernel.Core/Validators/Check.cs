using ManagementBE.Kernel.Core.Enums;
using ManagementBE.Kernel.Core.Wrappers;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace ManagementBE.Kernel.Core.Validators
{
    public static class Check
    {
        public static async Task<Response<bool>> ValidateAsync<T>(T dto, IValidatorFactory validatorFactory)
        {
            if (validatorFactory == null)
                throw new NullReferenceException("ValidatorFactory is null.");

            var validator = validatorFactory.GetValidator(dto.GetType());

            if (validator != null)
            {
                var context = new ValidationContext<T>(dto);
                var validationResult = validator.ValidateAsync(context);

                if (!validationResult.Result.IsValid)
                {
                    var resultValidation = await new ValidationProcessor<bool>().ProcessValidationResultOnFailureAsync(validationResult.Result);

                    return new Response<bool>()
                    {
                        StatusCode = (int)HttpStatusCode.BusinessRuleViolation,
                        BrokenRules = resultValidation.BrokenRules,
                    };
                }

                return new Response<bool>(true);
            }
            return new Response<bool>(true);
        }
    }
}
