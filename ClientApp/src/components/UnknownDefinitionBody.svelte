<script lang="ts">
    import DefinitionHeader from "./DefinitionHeader.svelte";
    import type {UnknownDefinition} from "$lib/generated/client";
    

    export let definition: UnknownDefinition
</script>

<DefinitionHeader lemma={definition.lemma} wordClass="unknown"/>
