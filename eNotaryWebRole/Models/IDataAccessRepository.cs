using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using eNotaryWebRole.ViewModel;

namespace eNotaryWebRole.Models
{
    public interface IDataAccessRepository
    {
         string getRole(string username);
         Act create_Act(long act_type_id, string act_name, string act_reason, string act_reason_state, string state, string act_extra_details, string username);
         void update_Act( long id,long act_type_id, string act_name,  string act_reason, string act_reason_state, string state, string act_extra_details, string username);
         void update_SignedAct(long id, long act_type_id, string act_name, string act_reason_signed,  bool act_sent_to_client, bool act_signed, string act_extra_details, string act_reason, string username);
        SignedAct create_SignedAct(long id, long act_type_id, string act_name, string act_reason_signed, bool act_sent_to_client, bool act_signed, string act_extra_details, string act_reason, string unique_ref, string username);
         DivorcePersonDetailViewModel create_person(string name, string father_name, string mother_name, string city, string county,
           string serie_act, string serie_no, string sns, string address, DateTime birthday);
}
}