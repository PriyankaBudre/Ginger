import { Component, Inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';


@Component({
  selector: 'services-grid-data',
  templateUrl: './servicesGrid.component.html'
})


export class ServicesGridComponent
{
  public services: Service[];
  mBaseUrl: string;

  
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string)
  {    
    this.mBaseUrl = baseUrl;

    http.get<Service[]>(baseUrl + 'api/ServiceGrid/NodeList').subscribe(result => {
      this.services = result;
    }, error => console.error(error));

  }


}


interface Service {
  name: string;
  description: string;
  fileName: string;
  status: string;
  elapsed: number;  
}
