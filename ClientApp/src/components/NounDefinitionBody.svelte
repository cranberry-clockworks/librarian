<script lang="ts">
    import DefinitionHeader from "./DefinitionHeader.svelte";
    import type {NounDefinition} from "$lib/generated/client";
    
    export let definition: NounDefinition
</script>

<DefinitionHeader article={definition.article} lemma={definition.lemma} wordClass="noun"/>
