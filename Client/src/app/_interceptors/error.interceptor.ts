import { HttpErrorResponse, HttpEvent, HttpInterceptorFn } from '@angular/common/http';
import { Observable, catchError } from 'rxjs';
import { throwError } from 'rxjs';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { inject } from '@angular/core';

export function errorInterceptor(req : any, next : any): Observable<any> {
    const router = inject(Router);
    const toastr = inject(ToastrService);
    return next(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error) {
          switch (error.status) {
            case 400:
              if (error.error.errors) {
                const modalStateErrors: string[] = [];
                for (const key in error.error.errors) {
                  if (error.error.errors[key]) {
                    modalStateErrors.push(error.error.errors[key])
                  }
                }
                throw modalStateErrors.flat();
              } else {
                toastr.error(error.statusText, error.status.toString());
              }
              break;
            case 401:
              toastr.error(error.statusText, error.status.toString());
              break;
            case 404:
              router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = { state: { error: error.error } };
              router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              toastr.error('Something unexpected went wrong');
              console.log(error);
              break;
          }
        }
        return throwError(error);
      })
    );
  };
