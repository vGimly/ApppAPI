import uslugi from './uslugi.js'
import tariffs from './tariffs.js'
import counters from './counters.js'
import measures from './measures.js'

export default {
data() { return {
	usluga: null,
	counter: null, counter_obj: null,
	showNotify: false,
}},
methods: {
    uslugaEvent (value) { this.usluga = value; this.counter=null; },
    counterEvent (value) {
		this.counter_obj = value;
		this.counter=value ? value.counterId: null;
		if (!this.usluga) this.usluga=value.uslugaRef;
    },
    format_def(value) { return isNaN(value)?'':this.counter_obj? this.format(value, this.counter_obj.precise, this.counter_obj.digits) : value},
//  format(value,prec,dig){return isNaN(value)?'':new Intl.NumberFormat('en-US', { minimumFractionDigits: prec, maximumFractionDigits: prec }).format(typeof(value)==='number'?value:parseFloat(value.replace(/,/g,''))).padStart(dig+prec+1,'0')},
    format(value,prec,dig){return isNaN(value)?'':Number.parseFloat(value).toFixed(prec).padStart(dig+prec+1,'0')},
    get_format(prec,dig) { if (this.counter_obj) { dig||=this.counter_obj.digits;prec||=this.counter_obj.precise} return prec && dig? '0'.padStart(dig,'0') + '.'.padEnd(prec,'0')+'1' :'10.10'},
    get_step(prec) { if (this.counter_obj) { prec||=this.counter_obj.precise} return prec? '0.'+'1'.padStart(prec,'0') :'0.01'},
    alert(msg) {
	console.groupCollapsed('app-notify');
	console.log(new Error(msg).stack);
	console.groupEnd();
	this.$refs.msg.innerHTML=msg;
	this.showNotify=true;
    },
},

template: `

<div class="tile is-ancestor" id="container">
  <div class="tile is-vertical is-9">
    <div class="tile">
      <div class="tile is-parent is-vertical is-4">
        <article class="tile is-child is-primary">

<uslugi @selected="uslugaEvent" :usluga="usluga" />

        </article>
        <article class="tile is-child">

<tariffs :key="usluga" />

        </article>
      </div>

      <div class="tile is-parent">
        <article class="tile is-child">

<counters :key="usluga" @selected="counterEvent" :counter="counter" />

        </article>
      </div>
    </div>
  </div>

  <div class="tile is-parent is-5">
    <article class="tile is-child">
<measures :key="''+ counter + '-' + usluga" :counter="counter" :usluga="usluga" />
    </article>
  </div>
</div>

<div class="notification is-danger is-primary" :class="{'is-hidden': !showNotify}">
  <button class="delete" @click="showNotify=false"></button>
    <span ref=msg></span>
</div>

`,

components: {uslugi, tariffs, counters, measures},
}
