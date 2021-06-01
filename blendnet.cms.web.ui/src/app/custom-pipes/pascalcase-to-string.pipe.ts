import { Pipe, PipeTransform } from '@angular/core';
@Pipe({
  name: 'pascalToString'
})
export class PascalToStringPipe implements PipeTransform {
  transform(value: any) {   
    const result = value.replace( /([A-Z])/g, " $1" );
    const finalResult = result.charAt(0).toUpperCase() + result.slice(1);
    return finalResult;
  }
}