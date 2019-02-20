import { Component, Inject, ViewChild, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { DOCUMENT } from '@angular/common';
import { MatPaginator, MatSort, MatSortModule, MatTableDataSource } from '@angular/material';


@Component({
  selector: 'business-flows-data',
  templateUrl: './businessflows.component.html'
})


export class BusinessFlowsComponent implements OnInit, AfterViewInit {
  public businessflows: BusinessFlow[];
  public report: string;
  mHttp: HttpClient;
  mBaseUrl: string;
  displayedColumns = ['name', 'description', 'fileName', 'status', 'elapsed','run'];
  public dataSource = new MatTableDataSource<BusinessFlow>();

  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string, @Inject(DOCUMENT) document) {
    this.mHttp = http;
    this.mBaseUrl = baseUrl;
  }

  ngOnInit() {
    this.getAllBF();
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }
  public getAllBF = () => {
    this.mHttp.get<BusinessFlow[]>(this.mBaseUrl + 'api/BusinessFlow/BusinessFlows').subscribe(result => {
      this.businessflows = result;
      this.dataSource = new MatTableDataSource(result);      
      this.dataSource.sort = this.sort;
      this.dataSource.paginator = this.paginator;
    }, error => console.error(error));
  }

  public doFilter = (value: string) => {
    this.dataSource.filter = value.trim().toLocaleLowerCase();
  }

  public runFlow(BF: BusinessFlow, index) {
    var elem = document.getElementById(index);
    elem.children.namedItem("Run").setAttribute("style", "display:none;");
    elem.children.namedItem("img").removeAttribute("style");

    BF.status = "Running";
    BF.elapsed = -1;
    const req = this.mHttp.post<RunBusinessFlowResult>(this.mBaseUrl + 'api/BusinessFlow/RunBusinessFlow', {
      name: BF.name  //TODO: We send the BF name replace with BF.Guid
    })
      .subscribe(
        res => {
          // Once we get the response        
          BF.status = res.status;
          BF.elapsed = res.elapsed;
          // this.report = res.report;
          elem.children.namedItem("img").setAttribute("style", "display:none;");
          elem.children.namedItem("Run").removeAttribute("style");
        },
        err => {
          console.log("Error occured");
          BF.status = "Exception while run flow";
          elem.children.namedItem("img").setAttribute("style", "display:none;"); 
          elem.children.namedItem("Run").removeAttribute("style");
        }
      );

  }

  public flowReport(BF: BusinessFlow) {

  }

}



interface RunBusinessFlowResult {
  status: string;
  elapsed: number;
  report: string;
}

interface BusinessFlow {
  name: string;
  description: string;
  fileName: string;
  status: string;
  elapsed: number;
  run: string;
}
