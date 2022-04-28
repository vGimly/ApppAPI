export default { name: 'measures',
  props: {usluga: Number, counter: Number},
  data() {
    return {
//      UColumns: [ `counter_ref`,`m_id`,`m_date`,`value`,`digits`,`precise`,`init_value`,`tarif` ],
      UColumns: [ `mDate`,`value`,`init`,`tariff`,`money` ],
      selected: null,
      newDate: '',
      newValue: '',
      tariff: ' ...',
      money: 0,
      s_money: 0,
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
					const m=(j.value-prev)*j.tariff;
					j.init=this.$parent.format_def(prev);
					j.mDate=j.mDate.replace(/T.*$/,'');
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
		if (this.selected.mDate === this.newDate && !this.selected.tariff)
		    this.selected.tariff=res;
		})
	 .catch(a=>this.$parent.alert(a));
    },

    async fetchMoney(e) {
	fetch(`api/counter/${this.counter}/money`)
	 .then(res=>res.text())
	 .then(res=>this.s_money=res)
	 .catch(a=>this.$parent.alert(a));
    },

    async add(e) {
	const form=this.$refs.form || e.target.form || e.target;
	const query=Object.fromEntries(new FormData(form).entries());
	delete query.mId;
	this.showModal=false;
	fetch(form.action.replace(/\b\d+\b/,this.counter),{method: form.method, headers: {'Content-Type':'application/json'},
            body: JSON.stringify(query)})
	 .then(res => {const ct=res.headers.get('content-type');
		if(!(res.status === 200 || res.status === 201) || !ct || ct.indexOf('application/json') === -1)
		    return res.text();
		else return res.json()})
	 .then(j => { if (typeof j !== 'object') throw(j);
		this.selected={mId: j['ok']};
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
	fetch(foru.action+sel.mId,{method: 'PUT', headers: {'Content-Type':'application/json'},
            body: JSON.stringify(Object.fromEntries(new FormData(form).entries()))})
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
	fetch(foru.action+sel.mId,{method: 'DELETE'})
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
	var needCalc=!sel.mDate || sel.mDate !== this.newDate;
	sel.mDate=this.newDate;
	sel.value=this.$parent.format_def(this.newValue);

	if (full)
	sel.counterId=this.counter;

	if (needCalc) this.fetchPrice();
    },

    do_select(u) {
	this.selected=u;
	if (u){
	this.newValue=u.value;
	this.newDate=u.mDate;
	if (u.tariff) this.tariff=u.tariff;
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

    get_fmt(){
        return this.$parent && this.$parent.get_format(this.newPrec,this.newDig);
    },
    get_step(){
        return this.$parent && this.$parent.get_step(this.newPrec);
    },
  },

  mounted() {
    if (this.usluga && this.counter)
	this.fetchData();
    if (this.counter) this.fetchMoney();
  },

template: `
<div :class="{modal: true, 'is-active': showModal}">
  <div class="modal-background"></div>
  <div class="modal-card">
<form ref=foru action="api/measure/"></form>
<form ref=form @submit.prevent="add" method=POST action="api/counter/0/measure">
<input type=submit style="display:none" title="to-catch-enter"/>
<input type=hidden name="mId" :value="selected && selected.mId" />
<input type=hidden name="counterRef" :value="counter" />

    <header class="modal-card-head">
    <p class="modal-card-title">Показания счётчика</p>
    <button class="delete" aria-label="close" @click.prevent="showModal=false"></button>
    </header>

    <section class="modal-card-body">
        <table class="table">
        <tr><td><label for=m-date>Дата:</label></td><td><input id=m-date name=mDate v-model="newDate" type=date @change="fetchPrice"></td></tr>
        <tr><td><label for=value>Показания:</label></td><td><input id=value name=value v-model="newValue" ref=first type=number :format="get_fmt()" :step="get_step()"></td></tr>
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
  <p v-else>Total: {{ money }} Server: {{ s_money }}</p>
  <table v-if="!!data" class="table is-bordered is-hoverable" id="measures">
    <thead><tr><th v-for="key in UColumns">{{key}}</th></tr></thead>
    <tbody>
      <tr v-for="u in data" @click="select(u)" :key="u.mId" :class="{ 'is-selected': selected === u }">
        <td v-for="key in UColumns" :id="key">
          {{u[key]}}
        </td>
      </tr>
    </tbody>
</table>
</div>
`};
