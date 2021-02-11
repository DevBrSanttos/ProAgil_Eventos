//modulos
import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { HttpClientModule } from "@angular/common/http";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ModalModule } from 'ngx-bootstrap/modal';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

import { ToastrModule } from 'ngx-toastr';

//serviços
import { EventoService } from './_services/evento.service';

//componentes
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { DashboardComponent } from './dashboard/dashboard.component';
import { EventosComponent } from './eventos/eventos.component';
import { PalestrantesComponent } from './palestrantes/palestrantes.component';
import { ContatosComponent } from './contatos/contatos.component';
import { TituloComponent } from './_shared/titulo/titulo.component';


//pipe
import { DateTimeFormatPipePipe } from './_helps/DateTimeFormatPipe.pipe';



@NgModule({
   declarations: [
      AppComponent,
      NavComponent,
      DashboardComponent,
      EventosComponent,
      PalestrantesComponent,
      ContatosComponent,
      TituloComponent,
      DateTimeFormatPipePipe
   ],
   imports: [
	 BrowserModule,
	 BrowserAnimationsModule,
	 AppRoutingModule,
	 HttpClientModule,
	 FormsModule,//formularios,
	 ReactiveFormsModule,//validaçãodeformularios,
	 ToastrModule.forRoot({
	 timeOut:1000,
	 },
	 ),
	 //NGXBOOSTRAP,
	 ModalModule.forRoot(),
	 BsDropdownModule.forRoot(),
	 BsDatepickerModule.forRoot(),//campodedata,
	 TooltipModule.forRoot()
	],
   providers: [
      EventoService
   ],
   bootstrap: [
      AppComponent
   ]
})
export class AppModule { }
