import { normalizeArray } from "core/utils"

export const UPDATE_LAYOUT = "layout_update_layout"
export const UPDATE_MODE = "layout_update_mode"
export const SHOW = "layout_show"

// export const ONLY_SHOW = "layout_only_show"

export function updateLayout(layout) {
  return {
    type: UPDATE_LAYOUT,
    payload: layout
  }
}

export function show(thing, shown=true) {
  thing = normalizeArray(thing)
  return {
    type: SHOW,
    payload: {thing, shown}
  }
}

// Simple string key-store, used for
export function changeMode(thing, mode="") {
  thing = normalizeArray(thing)
  return {
    type: UPDATE_MODE,
    payload: {thing, mode}
  }
}


// export function onlyShow(thing, shown=true) {
//   thing = normalizeArray(thing)
//   if(thing.length < 2)
//     throw new Error("layoutActions.onlyShow only works, when `thing` is an array with length > 1")
//   return {
//     type: ONLY_SHOW,
//     payload: {thing, shown}
//   }
// }



