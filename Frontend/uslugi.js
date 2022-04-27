export default { name: 'uslugi',
  props: {usluga: Number},
  emits: [ 'selected' ],
  data() {
    return {
//    UColumns: [ 'usluga_id', 'usluga_name' ],
      UColumns: [ 'uslugaName' ],
      newName: '',
      selected: null,
      showModal: false, timer: null, delay: 400,
      data: null
    }
  },
  methods: {
    notifyParent(){
	this.$emit('selected',this.selected && this.selected.uslugaId);
    },
    select(u) {
        if (this.timer && this.selected === u) {
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
	this.notifyParent();
	if (u) this.newName=u.uslugaName;
    },
    fill(sel){
	sel.uslugaName=this.newName;
    },
    show_modal(){
	this.showModal=true;
	Vue.nextTick(()=>this.$refs.first.focus());
    },

    submit(e){
	if (this.selected) return this.update(e);
	else return this.add(e);
    },

// REST/CRUD
    async fetchData() {
	const form=this.$refs.form;
	fetch(form.action+`?usl=${this.usluga}`)
	 .then(res => res.json())
	 .then(js => this.data = js)
	 .catch(e=>this.$parent.alert(e));
    },

    async add(e) {
	const form=this.$refs.form || e.target.form || e.target;
	const query=Object.fromEntries(new FormData(form).entries());
	this.showModal=false;
	delete query.uslugaId;
	fetch(form.action,{method: form.method,  headers: {'Content-Type':'application/json'},
            body: JSON.stringify(query)})
	 .then(res => res.json())
	 .then(j => {
		this.selected={uslugaId: j['ok']};
		this.fill(this.selected);
		this.data.push(this.selected);
		this.notifyParent();
		})
	 .catch(e=>this.$parent.alert(e));
    },

    async update(e) {
	this.showModal=false;
	const form=this.$refs.form || e.target.form || e.target;
	const sel=this.selected;
	fetch(form.action+sel.uslugaId,{method: 'PUT', headers: {'Content-Type':'application/json'},
            body: JSON.stringify(Object.fromEntries(new FormData(form).entries()))})
	 .then(res => res.text())
	 .then(res => {if (!res.startsWith('OK')) throw res;
			this.fill(sel);
		})
	.catch(e=>this.$parent.alert(e));
    },

    async remove(e) {
	this.showModal=false;
	const form=this.$refs.form || e.target.form || e.target;
	const sel=this.selected;
	fetch(form.action+sel.uslugaId,{method: 'DELETE'})
	 .then(res => res.text() )
	 .then(res => { if (!res.startsWith('OK')) throw res;
			this.data = this.data.filter(a=>a!==sel);
			if (this.selected === sel) this.do_select(null);
		})
	 .catch(e=>this.$parent.alert(e));
    },
  },
  mounted() {
    this.fetchData()
  },

template: `
<div :class="{modal: true, 'is-active': showModal}">
  <div class="modal-background"></div>
  <div class="modal-card">
<form ref=form @submit.prevent="submit" method=POST action="api/usluga/">
    <input type=submit style="display:none" title="to-catch-enter"/>
    <input type=hidden name=uslugaId :value="selected && selected.uslugaId" />
    <header class="modal-card-head">
    <p class="modal-card-title">Услуга</p>
    <button class="delete" aria-label="close" @click.prevent="showModal=false"></button>
    </header>
    <section class="modal-card-body">
	<table class="table">
	<tr><td><label for=usluga-name>Наименовение услуги:</label></td><td><input id=usluga-name name=uslugaName v-model="newName" ref=first></td></tr>
	</table>
    </section>
	<footer class="modal-card-foot">
		<button class="button is-primary is-light" @click.prevent="add">Добавить Услугу</button>
		<button class="button is-success is-light" @click.prevent="update" :disabled='!selected'>Изменить Услугу</button>
		<button class="button is-danger is-light"  @click.prevent="remove" :disabled='!selected'>Удалить Услугу</button>
	</footer>
</form>
  </div>
</div>
<header class="card-header"><button class="button is-success is-rounded" :class="{'is-loading': !data}" @click.prevent="show_modal">+</button><p class="card-header-title">Услуги</p></header>

<div class="box card-content">
  <table v-if="!!data" class="table is-bordered is-hoverable" id="uslugi">
    <thead @click="do_select(null)"><tr><th v-for="key in UColumns">{{key}}</th></tr></thead>
    <tbody>
      <tr v-for="u in data" @click="select(u)" :key="u.uslugaId" :class="{ 'is-selected': u.uslugaId === usluga }">
        <td v-for="key in UColumns" :id="key">
          {{u[key]}}
        </td>
      </tr>
    </tbody>
</table>
</div>
`};