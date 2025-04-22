import type { App } from "vue";
import { FontAwesomeIcon } from "@fortawesome/vue-fontawesome";

import { library } from "@fortawesome/fontawesome-svg-core";
import {
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUpRightFromSquare,
  faBan,
  faCheck,
  faChessRook,
  faEdit,
  faGear,
  faKey,
  faLanguage,
  faPlus,
  faRobot,
  faRotate,
  faSave,
  faUser,
  faUsersGear,
  faVial,
} from "@fortawesome/free-solid-svg-icons";

library.add(
  faArrowRightFromBracket,
  faArrowRightToBracket,
  faArrowUpRightFromSquare,
  faBan,
  faCheck,
  faChessRook,
  faEdit,
  faGear,
  faKey,
  faLanguage,
  faPlus,
  faRobot,
  faRotate,
  faSave,
  faUser,
  faUsersGear,
  faVial,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
