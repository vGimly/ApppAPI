export const tabs = { name: 'tabs',
    template: `
    <div class="tabs">
              <ul>
                <li v-for="tab in tabs" :class="{ 'is-active': tab.isActive }">
                    <a :href="tab.href" @click="selectTab(tab)">{{ tab.name }}</a>
                </li>
              </ul>
    </div>

    <div class="tabs-details">
                <slot></slot>
    </div>
`,

    data() { return {tabs: [], selected: null } },
    emits: ['tabSelected'],
    methods: {
        selectTab(selectedTab) {
            this.tabs.forEach(tab => {
		var is= tab === selectedTab;
		var was=tab.isActive;
		tab.isActive=is;
		if (is) this.selected=tab;
		if (is && !was) this.notifyParent();
	    });
        },
	notifyParent() { var id=this.selected ? this.selected.id || this.selected.id: null; this.$emit('tabSelected',id); },
    },
    mounted() { this.notifyParent() }
};

export const tab = { name: 'tab',
    template: `<div v-if="isActive"><slot></slot></div>`,

    created() { this.$parent.tabs.push(this); },

    props: {
        name: { required: true },
        id: { required: false },
        selected: { default: false }
    },

    data() { return { isActive: false } },

    computed: {
	myId() { return this.id.toLowerCase().replace(/ /g, '-') },
        href() { return '#' + this.myId}
    },

    mounted() { this.isActive = this.selected || (location.hash === '#' + this.myId); if (this.isActive) this.$parent.selected=this;}
};
