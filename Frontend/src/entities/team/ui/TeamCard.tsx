import type { Team } from '../model/types';

interface Props {
  team: Team;
  isCurrentUser?: boolean;
}

export const TeamCard = ({ team, isCurrentUser }: Props) => (
  <div className={`team-card ${isCurrentUser ? 'team-card--current' : ''}`}>
    <div className="team-card__name">{team.teamName}</div>
    <div className={`team-card__state team-card__state--${team.state}`}>
      {team.state}
    </div>
    {isCurrentUser && <span className="team-card__badge">Вы</span>}
  </div>
);