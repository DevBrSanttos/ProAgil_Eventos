import { Component, OnInit } from '@angular/core';
import { FormControl, FormGroup, Validators } from '@angular/forms';
import { BsModalService } from 'ngx-bootstrap/modal';
import { Evento } from '../_models/Evento';
import { EventoService } from '../_services/evento.service';
//imports do Datepicker
import { BsLocaleService } from 'ngx-bootstrap/datepicker';
import { ptBrLocale } from 'ngx-bootstrap/locale';
import { defineLocale } from 'ngx-bootstrap/chronos';
defineLocale('pt-br', ptBrLocale); //definindo o datepicker para portugues


@Component({
  selector: 'app-Eventos',
  templateUrl: './Eventos.component.html',
  styleUrls: ['./Eventos.component.css']
})
export class EventosComponent implements OnInit {
 
  eventosFiltrados: Evento[] = [];
  eventos: Evento[] = [];
  evento!: Evento;
  modoSalvar = "";
  bodyDeletarEvento = "";
  imagemLargura = 50;
  imagemMargem = 2;
  mostrarImagem = false;
  registerForm!: FormGroup;

  _filtroLista: any;

  constructor(
    private eventoService: EventoService, //carregar URL do back-and
    private modalService: BsModalService,
    private localeService: BsLocaleService
    ) { 
      this.localeService.use('pt-br');
    }

  get filtroLista(): string{
    return this._filtroLista;
  }

  set filtroLista(value: string){    
    this._filtroLista = value;
    this.eventosFiltrados = this.filtroLista ? this.filtrarEvento(this.filtroLista) : this.eventos;
  }

  ngOnInit() {    
    this.getEventos();
    this.validation();
    
  }

  filtrarEvento(filtrarPor: string): Evento[]{
    filtrarPor = filtrarPor.toLocaleLowerCase();
    return this.eventos.filter(
      (evento: { tema: string; }) => evento.tema.toLocaleLowerCase().indexOf(filtrarPor) !== -1
    );      
  }

  // mostrar/esconder imagens
  alternarImagem(){
    this.mostrarImagem = !this.mostrarImagem;
  }

  //carregar todos os eventos
  getEventos(){
    this.eventoService.getAllEventos().subscribe((_eventos: Evento[]) => {
      this.eventos = _eventos;
      this.eventosFiltrados = this.eventos;      
    }, error =>{
      console.log("erro ao carregar os eventos: ", error);
    });    
  }

    //abrir template modal quando apertar no botao editar
    openModal(template: any){
      this.registerForm.reset();      
      template.show();
    }

    editarEvento(template: any, evento: Evento){
      this.modoSalvar = "put";
      this.openModal(template);
      this.evento = evento;
      this.registerForm.patchValue(this.evento);
    }

    novoEvento(template: any){
      this.modoSalvar = "post";
      this.openModal(template);
    }

    salvarAlteracao(template: any){      
      if(this.registerForm.valid){        
        if(this.modoSalvar === "post"){
          this.evento = Object.assign({}, this.registerForm.value);
          this.eventoService.postEvento(this.evento).subscribe(()  => {
              template.hide(); //fechar template
              this.getEventos(); // atualizar lista
            }, error =>{
              console.log("erro ao salvar: ", error);
            }
          );
        }else{
          this.evento = Object.assign({id: this.evento.id}, this.registerForm.value);
          this.eventoService.putEvento(this.evento).subscribe(()  => {              
              template.hide(); //fechar template
              this.getEventos(); // atualizar lista
            }, error =>{
              console.log("erro ao atualizar: ", error);
            }
          );
        }

      }
    }
    //deletar evento
    excluirEvento(evento: Evento, template: any) {
      this.openModal(template);
      this.evento = evento;
      this.bodyDeletarEvento = `Tem certeza que deseja excluir o Evento: ${evento.tema}, Código: ${evento.id}`;
    }

    confirmeDelete(template: any) {
      this.eventoService.deleteEvento(this.evento.id).subscribe(
        () => {
            template.hide();
            this.getEventos();
          }, error => {
            console.log("erro ao confirmar: ", error);
          }
      );
    }

    //fazer validação do formulario
    validation(){
      this.registerForm = new FormGroup({
        tema: new FormControl("", [Validators.required, Validators.minLength(4),  Validators.maxLength(50)]),
        local: new FormControl("", Validators.required),
        dataEvento: new FormControl("", Validators.required),
        qtdPessoas: new FormControl("", [Validators.required, Validators.max(120000)]),
        imagemURL: new FormControl("", Validators.required),
        telefone: new FormControl("", Validators.required),
        email: new FormControl("", [Validators.required, Validators.email])
      })
    }
}
