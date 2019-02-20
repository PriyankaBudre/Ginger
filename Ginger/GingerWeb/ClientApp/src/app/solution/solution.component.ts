import { Component, Inject, ViewChild, OnInit, AfterViewInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatPaginator, MatSort, MatSortModule, MatTableDataSource } from '@angular/material';


@Component({
  selector: 'app-solution',
  templateUrl: './solution.component.html'
})


export class SolutionComponent {
  public solutions: Solution[];
  public report: string;
  mHttp: HttpClient;
  mBaseUrl: string;
  displayedColumns = ['name', 'path', 'action'];
  public dataSource = new MatTableDataSource<Solution>();
  @ViewChild(MatPaginator) paginator: MatPaginator;
  @ViewChild(MatSort) sort: MatSort;
  constructor(http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.mHttp = http;
    this.mBaseUrl = baseUrl;
  }

  ngOnInit() {
    this.getAllSolutions();
  }

  ngAfterViewInit(): void {
    this.dataSource.sort = this.sort;
    this.dataSource.paginator = this.paginator;
  }
  public getAllSolutions = () => {
    this.mHttp.get<Solution[]>(this.mBaseUrl + 'api/Solution/Solutions').subscribe(result => {
      this.solutions = result;
      this.dataSource = new MatTableDataSource(result);
      this.dataSource.sort = this.sort;
      this.dataSource.paginator = this.paginator;
    }, error => console.error(error));
  }

  public doFilter = (value: string) => {
    this.dataSource.filter = value.trim().toLocaleLowerCase();
  }


  public openSolution(solution: Solution) {

    const req = this.mHttp.post<RunBusinessFlowResult>(this.mBaseUrl + 'api/Solution/OpenSolution', {
      name: solution.path
    })
      .subscribe(
        res => {
          // Once we get the response        
          //BF.status = res.status;
          //BF.elapsed = res.elapsed;
          // this.report = res.report;
        },
        err => {
          console.log("Error occured");
          //BF.status = "Error 123";
        }
      );
  }
}

interface RunBusinessFlowResult {
  status: string;
  elapsed: number;
  report: string;
}

interface Solution {
  name: string;
  path: string;  
}
