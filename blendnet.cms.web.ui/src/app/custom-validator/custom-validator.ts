import { regex } from "../constants/regex";

export class CustomValidator {

    static alphaNumeric(data): any {
        if(data.pristine) {
            return null;
        }
        data.markAsTouched();

        if(regex.alphaNumeric.test(data.value)) {
            return null;
        }

        return {
            invalid: true
        };
    }

    static alphaNumericSplChar(data): any {
        if(data.pristine) {
            return null;
        }
        data.markAsTouched();
        
        if(regex.alphaNumericLimitedSplChar.test(data.value)) {
            return null;
        }

        return {
            invalid: true
        };
    }

    static numeric(data): any {
        if(data.pristine) {
            return null;
        }
        data.markAsTouched();

        if(regex.numeric.test(data.value)) {
            return null;
        }

        return {
            invalid: true
        };
    }

    static url(data): any {
        if(data.pristine) {
            return null;
        }
        data.markAsTouched();

        if(regex.url.test(data.value)) {
            return null;
        }

        return {
            invalid: true
        };
    }
    
    
}