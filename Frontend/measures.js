export default { name: 'measures',
  props: {usluga: Number, counter: Number},
  data() {
    return {
//      UColumns: [ `counter_id`,`m_id`,`m_date`,`value`,`digits`,`precise`,`init_value`,`tarif` ],
      UColumns: [ `m_date`,`value`,`init`,`tarif`,`money` ],
      selected: null,
      newDate: '',
      newValue: '',
      tariff: ' ...',
      money: 0,
      showModal: false, timer: null, delay: 400,
      data: null
    }
  },
  methods: {

    async fetchData() {
	this.data = null;
	const form=this.$refs.form;
	fetch(form.action.replace(/\b\d+\b/,this.counter))
	 .then(res => res.json())
	 .then(js => { this.data = js;
			var prev=null;
			var money=0;
			js.forEach(j=>{ if (!prev) prev=j.value;
					const m=(j.value-prev)*j.tarif;
					j.init=this.$parent.format_def(prev);
					j.m_date=j.m_date.replace(/T.*$/,'');
					prev=j.value;
					money+=m;
					j.money=Math.floor(m*100+.5)/100;
					j.value=this.$parent.format_def(j.value)});
			this.money=Math.floor(money*100000+.5)/100000;
		  })
	 .catch(a=>this.$parent.alert(a));
    },

    async fetchPrice(e) {
	this.tariff=' ...';
	if (!this.newDate) return;
// ToDo: delay query
	fetch(`api/usluga/${this.usluga}/tariff/`+this.newDate)
	 .then(res => {const ct=res.headers.get('content-type');
		if(res.status !== 200 || !ct || ct.indexOf('html') !== -1)
		    return res.text().then(res => window.open("","err",`width=800,height=400,screenX=200,screenY=200`).document.body.innerHTML=res );
		return res.text()})
	 .then(res=>{ this.tariff=res;
		if (this.selected.m_date === this.newDate && !this.selected.tarif)
		    this.selected.tarif=res;
		})
	 .catch(a=>this.$parent.alert(a));
    },

    async add(e) {
	const form=this.$refs.form || e.target.form || e.target;
	this.showModal=false;
	fetch(form.action.replace(/\b\d+\b/,this.counter),{method: form.method, body: new FormData(form)})
	 .then(res => {const ct=res.headers.get('content-type');
		if(!(res.status === 200 || res.status === 201) || !ct || ct.indexOf('application/json') === -1)
		    return res.text();
		else return res.json()})
	 .then(j => { if (typeof j !== 'object') throw(j);
		this.selected={m_id: j['OK']||j['ok']};
		this.fill(this.selected,true);
		this.data.push(this.selected);
		})
	 .catch(a=>this.$parent.alert(a));
    },

    async update(e) {
	const form=this.$refs.form || e.target.form || e.target;
	const sel=this.selected;
	const foru=this.$refs.foru;
	this.showModal=false;
	fetch(foru.action+sel.m_id,{method: 'PUT', body: new FormData(form)})
	 .then(res => res.text())
	 .then(res => { if (!res.startsWith('OK')) throw(res);
		    this.fill(sel,false);
		})
	 .catch(a=>this.$parent.alert(a));
    },

    async remove(e) {
	const form=this.$refs.form || e.target.form;
	const sel=this.selected;
	const foru=this.$refs.foru;
	this.showModal=false;
	fetch(foru.action+sel.m_id,{method: 'DELETE'})
	 .then(res => res.text() )
	 .then(res => { if (!res.startsWith('OK')) throw(res)
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
	    this.timer=setTimeout(()=>this.timer=null, this.delay);
	    this.do_select(u);
	}
    },

    fill(sel,full){
	var needCalc=!sel.m_date || sel.m_date !== this.newDate;
	sel.value=this.$parent.format_def(this.newValue);
	sel.m_date=this.newDate;

	if (full)
	sel.counter_id=this.counter;

	if (needCalc) this.fetchPrice();
    },

    do_select(u) {
	this.selected=u;
	if (u){
	this.newValue=u.value;
	this.newDate=u.m_date;
	if (u.tarif) this.tariff=u.tarif;
	else this.fetchPrice();
	}
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
    if (this.usluga && this.counter)
	this.fetchData();
  },

template: `
<div :class="{modal: true, 'is-active': showModal}">
  <div class="modal-background"></div>
  <div class="modal-card">
<form ref=foru action="api/measure/"></form>
<form ref=form @submit.prevent="add" method=POST action="api/counter/0/measure">
<input type=submit style="display:none" title="to-catch-enter"/>

    <header class="modal-card-head">
    <p class="modal-card-title">Показания счётчика</p>
    <button class="delete" aria-label="close" @click.prevent="showModal=false"></button>
    </header>

    <section class="modal-card-body">
        <table class="table">
        <tr><td><label for=m-date>Дата:</label></td><td><input name=m-date v-model="newDate" type=date @change="fetchPrice"></td></tr>
        <tr><td><label for=value>Показания:</label></td><td><input ref=first id=value name=value v-model="newValue"></td></tr>
	<tr><td>Действущий тариф:</td><td title="Выбрать дату">{{ tariff }}</td></tr>
        </table>
    </section>
	<footer class="modal-card-foot">
	    <button class="button is-primary is-light" @click.prevent="add">Добавить Показания</button>
	    <button class="button is-success is-light" @click.prevent="update" :disabled='!selected'>Изменить Показания</button>
	    <button class="button is-danger is-light"  @click.prevent="remove" :disabled='!selected'>Удалить Показания</button>
	</footer>
</form>
  </div>
</div>

<header class="card-header"><button class="button is-success is-rounded" :class="{'is-loading': !data && (usluga && counter)}" @click.prevent="show_modal" :disabled="!(usluga && counter)"
    title="Для добавления выбрать счётчик. Редактирование - двойной клик.">+</button><p class="card-header-title">Показания</p></header>
<div class="box card-content">
  <p v-if="!data && !(usluga && counter)">Выбрать услугу и счётчик...</p>
  <p v-else>Total: {{ money }}</p>
  <table v-if="!!data" class="table is-bordered is-hoverable" id="measures">
    <thead><tr><th v-for="key in UColumns">{{key}}</th></tr></thead>
    <tbody>
      <tr v-for="u in data" @click="select(u)" :key="u.m_id" :class="{ 'is-selected': selected === u }">
        <td v-for="key in UColumns" :id="key">
          {{u[key]}}
        </td>
      </tr>
    </tbody>
</table>
</div>
`};
