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
  faHourglassHalf,
  faKey,
  faLanguage,
  faPlus,
  faRobot,
  faRotate,
  faSave,
  faStar,
  faTimes,
  faUser,
  faUsers,
  faUserClock,
  faUsersGear,
  faVial,
  faUserCheck,
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
  faHourglassHalf,
  faKey,
  faLanguage,
  faPlus,
  faRobot,
  faRotate,
  faSave,
  faStar,
  faTimes,
  faUser,
  faUserCheck,
  faUserClock,
  faUsers,
  faUsersGear,
  faVial,
);

export default function (app: App) {
  app.component("font-awesome-icon", FontAwesomeIcon);
}
