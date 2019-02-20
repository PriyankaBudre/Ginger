import { Component, Inject, ViewChild, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatPaginator, MatSort, MatSortModule, MatTableDataSource } from '@angular/material';


@Component({
  selector: 'services-grid-data',
  templateUrl: './servicesGrid.component.html'
})


export class ServicesGridComponent {
  public services: Service[];
  mHttp: HttpClient;
  mBaseUrl: string;
  displayedColumns = ['name', 'serviceId', 'sessionId', 'host', 'ip', 'os', 'status', 'actionCount'];
  public dataSource = new MatTableDataSource<Service>();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;


  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.mHttp = http;
    this.mBaseUrl = baseUrl;
  }
  ngOnInit() {
    this.getAllSG();
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }
  public getAllSG = () => {

    this.mHttp.get<Service[]>(this.mBaseUrl + 'api/ServiceGrid/NodeList').subscribe(result => {
      this.services = result;
      this.dataSource = new MatTableDataSource(result);
      this.dataSource.sort = this.sort;
      this.dataSource.paginator = this.paginator;
    }, error => console.error(error));
  }

  public doFilter = (value: string) => {
    this.dataSource.filter = value.trim().toLocaleLowerCase();
  }


}


interface Service {
  name: string;
  description: string;
  fileName: string;
  status: string;
  elapsed: number;
}
