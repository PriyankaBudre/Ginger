import { Component, Inject, ViewChild, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatPaginator, MatSort, MatSortModule, MatTableDataSource } from '@angular/material';

@Component({
  selector: 'runset',
  templateUrl: './runset.component.html'
})

export class RunSetComponent {
  public runsets: RunSet[];
  mHttp: HttpClient;
  mBaseUrl: string;
  displayedColumns = ['name', 'description', 'status', 'elapsed', 'run'];
  public dataSource = new MatTableDataSource<RunSet>();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;

  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.mHttp = http;
    this.mBaseUrl = baseUrl;
  }

  ngOnInit() {
    this.getAllRS();
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }
  public getAllRS = () => {

    this.mHttp.get<RunSet[]>(this.mBaseUrl + 'api/RunSet/RunSets').subscribe(result => {
      this.runsets = result;
      this.dataSource = new MatTableDataSource(result);
      this.dataSource.sort = this.sort;
      this.dataSource.paginator = this.paginator;
    }, error => console.error(error));
  }

  public doFilter = (value: string) => {
    this.dataSource.filter = value.trim().toLocaleLowerCase();
  }

  public run(runset: RunSet) {

    runset.status = "Running";
    runset.elapsed = -1;
    const req = this.mHttp.post<RunSetResult>(this.mBaseUrl + 'api/RunSet/RunRunSet', {
      name: runset.name  //TODO: We send the runset name replace with runset.Guid
    })
      .subscribe(
        res => {
          // Once we get the response        
          runset.status = res.status;
          runset.elapsed = res.elapsed;
          // this.report = res.report;
        },
        err => {
          console.log("Error occured");
          runset.status = "Error !!!";
        }
      );
  }
}

interface RunSetResult {
  status: string;
  elapsed: number;
  report: string;
}

interface RunSet {
  name: string;
  description: string;
  fileName: string;
  status: string;
  elapsed: number;
}
