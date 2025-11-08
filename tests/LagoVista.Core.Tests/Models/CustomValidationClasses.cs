// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: 2c169eaa19a69b269a1b849d5c22ba3366d5c371d7a349c5ef63c3bcb4597a24
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.Attributes;
using LagoVista.Core.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LagoVista.Core.Validation.Validator;

namespace LagoVista.Core.Tests.Models
{
    public class CustomValidation_ValidModel : IValidateable
    {       

        public int Called = 0;

        [CustomValidator]
        public void CustomValidator(ValidationResult result)
        {
            Called++;
        }
    }

    public class CustomValidation_BothParameter_Valid : IValidateable
    {
        public int Called = 0;

        [CustomValidator]
        public void CustomValidator(ValidationResult result, Actions action)
        {
            Called++;
        }
    }

    public class CustomValidation_TwoMethods_Valid : IValidateable
    {
        public int Called = 0;

        [CustomValidator]
        public void CustomValidator(ValidationResult result)
        {
            Called++;
        }

        [CustomValidator]
        public void CustomValidator2(ValidationResult result, Actions action)
        {
            Called++;
        }
    }


    public class CustomValidation_NoParameter_Invalid : IValidateable
    {
        [CustomValidator]
        public void CustomValidator()
        {

        }
    }

    public class CustomValidation_NoCustomValidation_Valid : IValidateable
    {
        public int Called = 0;
        public String CustomValidator(String foo, String fee)
        {
            Called++;
            return "DONT CARE";
        }
    }


    public class CustomValidation_WrongType_Invalid : IValidateable
    {
        [CustomValidator]
        public void CustomValidator(String wrongValue)
        {

        }
    }

    public class CustomValidation_WrongSecondType_Invalid : IValidateable
    {
        [CustomValidator]
        public void CustomValidator(ValidationResult wrongValue, String action)
        {

        }
    }

    public class CustomValidation_TooManyTypes_Invalid : IValidateable
    {
        [CustomValidator]
        public void CustomValidator(ValidationResult result, Actions action, String moreInfo)
        {

        }
    }

    public class CustomValidation_TestOnInsert : IValidateable
    {
        public int Called = 0;
        public String Field { get; set; }

        [CustomValidator]
        public void CustomValidator(ValidationResult result, Actions action)
        {
            Called++;
            if (String.IsNullOrEmpty(Field) && action == Actions.Create)
            {
                result.Errors.Add(new ErrorMessage("ERROR", true));
            }
        }
    }

    public class CustomValidation_TestOnUpdate: IValidateable
    {
        public int Called = 0;
        public String Field { get; set; }

        [CustomValidator]
        public void CustomValidator(ValidationResult result, Actions action)
        {
            Called++;
            if (String.IsNullOrEmpty(Field) && action == Actions.Update)
            {
                result.Errors.Add(new ErrorMessage("ERROR", true));
            }
        }
    }
}
