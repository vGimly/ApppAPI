export default { name: 'tariffs',
  data() {
    return {
//      UColumns: [ 'tariff_id', 'usluga_ref', 't_date', 'price' ],
      UColumns: [ 'TDate', 'price' ],
      selected: null,
      usluga: this.$.vnode.key,
      newDate: '',
      newPrice: '',
      showModal: false, timer: null, delay: 400,
      data: null
    }
  },
  methods: {
    async fetchData() {
	this.data = null;
	const form=this.$refs.form;
	fetch(form.action.replace(/\b\d+\b/,this.usluga))
	 .then(res => res.json())
	 .then(js => {
		this.data = js;
		js.forEach(j=>j.TDate=j.TDate.replace(/T.*$/,''));
	})
	 .catch(e=>this.$parent.alert(e));
    },

    async add(e) {
	const form=this.$refs.form || e.target.form || e.target;
	this.showModal=false;
	if (!this.usluga) return alert('Для добавления тарифа выберите услугу')
	fetch(form.action.replace(/\b\d+\b/,this.usluga),{method: form.method,  headers: {'Content-Type':'application/json'},
            body: JSON.stringify(Object.fromEntries(new FormData(form).entries()))})
         .then(res => {const ct=res.headers.get('content-type');
                if(!(res.status === 200 || res.status === 201) || !ct || ct.indexOf('application/json') === -1)
                    return res.text();
                else return res.json()})
         .then(j => { if (typeof j !== 'object') throw(j);
		this.selected={TariffId: j['OK']||j['ok']};
		this.fill(this.selected,true);
		this.data.push(this.selected);
                })
         .catch(a=>this.$parent.alert(a));
    },

    async update(e) {
	const form=this.$refs.form || e.target.form || e.target;
	const foru=this.$refs.foru;
	const sel=this.selected;
	this.showModal=false;
	fetch(foru.action + sel.TariffId,{method: 'PUT', headers: {'Content-Type':'application/json'},
            body: JSON.stringify(Object.fromEntries(new FormData(form).entries()))})
	 .then(res => res.text())
	 .then(res => {if (!res.startsWith('OK')) throw res;
		this.fill(sel)
	    })
	 .catch(e=>this.$parent.alert(e));
    },
    async remove(e) {
//	const form=this.$refs.form || e.target.form || e.target;
	const sel=this.selectd;
	const foru=this.$refs.foru;
	this.showModal=false;
	fetch(foru.action + sel.TariffId,{method: 'DELETE'})
	 .then(res => res.text() )
	 .then(res => { if (!res.startsWith('OK')) throw res;
			this.data = this.data.filter(a=>a!==sel);
			if (this.selected === sel) this.do_select(null);
	 })
	 .catch(e=>this.$parent.alert(e));
    },

    show_modal(){
       this.showModal=true;
       Vue.nextTick(()=>this.$refs.first.focus());
    },

    submit(e){
       if (this.selected) return this.update(e);
       else return this.add(e);
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
    do_select(u) {
	this.selected=u;
	if (u){
	this.newDate=u.TDate;
	this.newPrice=u.Price;
	}
    },
    fill(sel,full){
		sel.TDate=this.newDate;
		sel.Price=this.newPrice;
	if (full) sel.UslugaRef=this.usluga;
    },
  },
  mounted() { this.fetchData() },

template: `
<div :class="{modal: true, 'is-active': showModal}">
  <div class="modal-background"></div>
  <div class="modal-card">
<form ref=foru action="api/tariff/"></form>
<form ref=form @submit.prevent="submit" method=POST action="api/usluga/0/tariff">
<input type=submit style="display:none" title="to-catch-enter"/>
<input type=hidden name=TariffId :value="selected && selected.TariffId" />
<input type=hidden name=UslugaRef :value=usluga />
    <header class="modal-card-head">
    <p class="modal-card-title">Тариф</p>
    <button class="delete" aria-label="close" @click.prevent="showModal=false"></button>
    </header>
    <section class="modal-card-body">
	<table class="table">
	<tr><td><label for=tariff-start>Начало действия:</label></td><td><input id=tariff-start name=TariffStart type=date v-model=newDate></td></tr>
	<tr><td><label for=tariff-value>Значение тарифа:</label></td><td><input id=tariff-value name=TariffValue v-model=newPrice type=number format='10.00' step='0.01' ref=first></td></tr>
	</table>
    </section>
	<footer class="modal-card-foot">
		<button class="button is-primary is-light" @click.prevent="add">Добавить Тариф</button>
		<button class="button is-success is-light" @click.prevent="update" :disabled='!selected'>Изменить Тариф</button>
		<button class="button is-danger is-light"  @click.prevent="remove" :disabled='!selected'>Удалить Тариф</button>
        </footer>
</form>
  </div>
</div>

<header class="card-header"><button class="button is-success is-rounded" :class="{'is-loading': !data}" @click.prevent="show_modal" :disabled="!usluga"
    title="Для добавления выбрать услугу. Редактирование - двойной клик.">+</button><p class="card-header-title">Тарифы</p></header>
<div class="box card-content">
  <table v-if="!!data" class="table is-bordered is-hoverable" id="tariff">
    <thead><tr><th v-for="key in UColumns">{{ key }}</th></tr></thead>
    <tbody>
	<tr v-for="u in data" :key="u.TariffId" :class="{ 'is-selected': u === selected }" @click="select(u)">
	    <td v-for="key in UColumns">{{ u[key] }}</td>
	</tr>
    </tbody>
</table>
</div>
`};
