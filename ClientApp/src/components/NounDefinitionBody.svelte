<script lang="ts">
    import type {NounDefinition} from "$lib/generated/client";
    import DefinitionTitle from "./DefinitionTitle.svelte";

    export let definition: NounDefinition
</script>

<DefinitionTitle article={definition.article} lemma={definition.lemma} wordClass="noun"/>
