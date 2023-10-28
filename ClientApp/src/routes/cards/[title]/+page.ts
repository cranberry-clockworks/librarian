import type { Load } from "@sveltejs/kit"

export const load: Load = ({ params }) : CardPageParameters => {
    return {
        title: params.title ?? ""
    }
};

export type CardPageParameters = {
    title: string;
}