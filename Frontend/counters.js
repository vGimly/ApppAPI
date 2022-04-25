export default { name: 'counters',
  props: {counter: Number},
  data() {
    return {
//      UColumns: [ `counter_id`,`usluga_ref`,`counter_name`,`serial`,`digits`,`precise`,`start_date`,`init_value` ],
//      UColumns: [ `counter_name`,`serial`,`digits`,`precise` ],
      UColumns: [ `counter_name`,`serial`,`digits`,`precise`,`start_date`,`init_value` ],
      usluga: this.$.vnode.key,
      selected: null,
      newName: '',
      newSer: '',
      newDig: '',
      newPrec: '',
      newStart: '',
      newInit: '',
      showModal: false, timer: null, delay: 400,
      data: null
    }
  },
  emits: ['selected'],
  methods: {
    notifyParent(){this.$emit('selected',this.selected)},
    async fetchData() {
	this.data = null;
	const form=this.$refs.form;
	fetch(form.action.replace(/\b\d+\b/,this.usluga))
	 .then(res => res.json())
	 .then(js => {this.data = js;
		js.forEach(c=>{
		    if (!this.selected && this.counter && c.counter_id === this.counter)
			this.selected=c;
		    c.init_value=this.$parent.format(c.init_value,c.precise,c.digits);
		});
	    })
	 .catch(a=>this.$parent.alert(a));
    },

    async add(e) {
	const form=this.$refs.form || e.target.form || e.target;
	this.showModal=false;
	fetch(form.action.replace(/\b\d+\b/,this.usluga),{method: form.method, body: new FormData(form)})
	 .then(res => {const ct=res.headers.get('content-type');
		if(!(res.status === 200 || res.status === 201) || !ct || ct.indexOf('application/json') === -1)
		  return res.text();
		else return res.json()})
	 .then(j => { if (typeof j !== 'object') throw(j);
		this.selected={counter_id: j['OK']||j['ok']};
		this.fill(this.selected,true);
		this.data.push(this.selected);
		this.notifyParent();
		})
	 .catch(a=>this.$parent.alert(a));
    },

    async update(e) {
	const form=this.$refs.form || e.target.form;
	const foru=this.$refs.foru;
	const sel=this.selected;
	this.showModal=false;
	fetch(foru.action+sel.counter_id,{method: 'PUT', body: new FormData(form)})
	 .then(res => res.text())
	 .then(res => {if (!res.startsWith('OK')) throw(res);
		this.fill(sel,false);
		})
	 .catch(a=>this.$parent.alert(a));
    },

    async remove(e) {
	const form=this.$refs.form || e.target.form;
	const foru=this.$refs.foru;
	const sel=this.selected;
	this.showModal=false;
	fetch(foru.action+sel.counter_id,{method: 'DELETE'})
	 .then(res => res.text() )
	 .then(res => { if (!res.startsWith('OK')) throw(res);
			this.data = this.data.filter(a=>a!==sel);
			if (sel === this.selected) this.do_select(null);
	    })
	 .catch(a=>this.$parent.alert(a));
    },

    select(u) {
        if (this.timer && this.selected === u)
        {
	    clearTimeout(this.timer);
	    this.timer=null;
	    this.show_modal();
	}
        else {
            this.timer=setTimeout(()=>{this.timer=null}, this.delay);
            this.do_select(u);
        }
    },

    fill(sel,full){
	sel.counter_name=this.newName;
	sel.serial=this.newSer;
	sel.digits=this.newDig;
	sel.precise=this.newPrec;
	if (full){
	sel.start_date=this.newStart;
	sel.init_value=this.$parent.format(this.newInit, this.newPrec, this.newDig);
	sel.usluga_ref=this.usluga;
	}},

    do_select(u) {
	this.selected=u;
	if (u){
	this.newName=u.counter_name;
	this.newSer=u.serial;
	this.newDig=u.digits;
	this.newPrec=u.precise;
	this.newStart=u.start_date;
	this.newInit=u.init_value;
	}
	this.notifyParent();
    },

    show_modal(){
	this.showModal=true;
	Vue.nextTick(()=>this.$refs.first.focus());
    },

    submit(e){
	if (this.selected) return this.update(e);
	else return this.add(e);
    },

  },
  mounted() {
    this.fetchData()
  },

template: `
<div :class="{modal: true, 'is-active': showModal}">
  <div class="modal-background"></div>
  <div class="modal-card">
<form ref=foru action="api/counter/"></form>
<form ref=form @submit.prevent="submit" method=POST action="api/usluga/0/counter">
<input type=submit style="display:none" title="to-catch-enter"/>

    <header class="modal-card-head">
    <p class="modal-card-title">Счётчик</p>
    <button class="delete" aria-label="close" @click.prevent="showModal=false"></button>
    </header>

    <section class="modal-card-body">
        <table class="table">
        <tr><td><label for=counter-name>Наименовение счётчика:</label></td><td><input ref=first id=counter-name name=counter-name v-model="newName"></td></tr>
        <tr><td><label for=serial>Серийный номер:</label></td><td><input id=serial name=serial v-model="newSer"></td></tr>
        <tr><td><label for=digits>Разрядность:</label></td><td><input id=digits name=digits v-model="newDig"></td></tr>
        <tr><td><label for=precise>Точность:</label></td><td><input id=precise name=precise v-model="newPrec"></td></tr>
        <tr><td><label for=start-date>Начало работы:</label></td><td><input id=start-date name=start-date v-model="newStart" type=date></td></tr>
        <tr><td><label for=init-value>Начальные показания:</label></td><td><input id=init-value name=init-value v-model="newInit"></td></tr>
        </table>
    </section>
        <footer class="modal-card-foot">
            <button class="button is-primary is-light" @click.prevent="add">Добавить Счётчик</button>
            <button class="button is-success is-light" @click.prevent="update" :disabled='!selected'>Изменить Счётчик</button>
            <button class="button is-danger is-light"  @click.prevent="remove" :disabled='!selected'>Удалить Счётчик</button>
        </footer>
</form>
  </div>
</div>

<header class="card-header"><button class="button is-success is-rounded" :class="{'is-loading': !data}" @click.prevent="show_modal" :disabled="!usluga"
    title="Для добавления выбрать услугу. Редактирование - двойной клик.">+</button><p class="card-header-title">Счётчики</p></header>
<div class="box card-content">
  <table v-if="!!data" class="table is-bordered is-hoverable" id="counters">
    <thead><tr><th v-for="key in UColumns">{{key}}</th></tr></thead>
    <tbody>
      <tr v-for="u in data" @click="select(u)" :key="u.counter_id" :class="{ 'is-selected': selected === u }">
        <td v-for="key in UColumns" :id="key">
          {{u[key]}}
        </td>
      </tr>
    </tbody>
</table>
</div>

`};
