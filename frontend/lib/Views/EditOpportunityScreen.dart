import 'package:flutter/material.dart';
import 'package:frontend/Enums/OppCategory.dart';
import 'package:frontend/Services/opportunity_api_handler.dart';
import 'package:frontend/Views/OpportunityManager.dart';
import 'package:frontend/Models/Opportunity.dart';
import 'package:frontend/Components/CustomAppBar.dart';
import 'package:frontend/Components/CustomDrawer.dart';
import 'package:frontend/Enums/Location.dart';
import 'package:frontend/Models/OpportunityImg.dart';
import 'package:provider/provider.dart';
import 'package:frontend/Components/DynamicTextField.dart';
import 'package:frontend/Components/OpportunityImagePicker.dart';
import 'package:frontend/Views/OpportunityDetailsScreen.dart';
import 'package:frontend/Components/OpportunityDateTimePicker.dart';

class EditOpportunityScreen extends StatefulWidget {
  final Opportunity opportunity;

  const EditOpportunityScreen({super.key, required this.opportunity});

  @override
  _EditOpportunityScreenState createState() => _EditOpportunityScreenState();
}

class _EditOpportunityScreenState extends State<EditOpportunityScreen> {
  late ScrollController verticalScrollController;
  late TextEditingController nameController;
  late TextEditingController descriptionController;
  late TextEditingController addressController;
  late TextEditingController vacanciesController;
  late TextEditingController priceController;

  DateTime? _oppDateController;
  TimeOfDay? _oppTimeController;
  List<OpportunityImg> _opportunityImgs = [];
  late Location selectedLocation;
  late OppCategory selectedCategory;
  String _errorMessage = "";

  @override
  void initState() {
    super.initState();
    verticalScrollController = ScrollController();

    // Initialize controllers with the current values from the Opportunity object
    nameController = TextEditingController(text: widget.opportunity.name);
    descriptionController = TextEditingController(text: widget.opportunity.description);
    addressController = TextEditingController(text: widget.opportunity.address);
    vacanciesController = TextEditingController(text: widget.opportunity.vacancies.toString());
    priceController = TextEditingController(text: widget.opportunity.price.toString());
    selectedLocation = widget.opportunity.location;
    selectedCategory = widget.opportunity.category;
  }

  @override
  void dispose() {
    verticalScrollController.dispose();
    nameController.dispose();
    addressController.dispose();
    descriptionController.dispose();
    vacanciesController.dispose();
    priceController.dispose();
    super.dispose();
  }

  Future<void> _saveOpportunity() async {

    if (_opportunityImgs.length > 5) {
      setState(() {
        _errorMessage = 'Não é permitido adicionar mais de 5 imagens';
      });
      return null;
    }

    final List<OpportunityImg> finalImages = _opportunityImgs.isNotEmpty
      ? _opportunityImgs
      : widget.opportunity.opportunityImgs;

    Opportunity updatedOpportunity = Opportunity(
      opportunityId: widget.opportunity.opportunityId,
      name: nameController.text, 
      price: double.tryParse(priceController.text) ?? 0.0, 
      vacancies: int.tryParse(vacanciesController.text) ?? 0, 
      isActive: widget.opportunity.isActive, 
      category: selectedCategory, 
      description: descriptionController.text, 
      location: selectedLocation, 
      address: addressController.text, 
      userId: widget.opportunity.userId, 
      reviewScore: widget.opportunity.reviewScore, 
      date: (_oppDateController ?? DateTime.now()).add(Duration(days: 1)), 
      isImpulsed: widget.opportunity.isImpulsed, 
      opportunityImgs: finalImages);

    
    final success = await Provider.of<OpportunityApiHandler>(context, listen: false)
      .editOpportunity(widget.opportunity.opportunityId, updatedOpportunity);
    
    if (success) {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Alterações guardadas com sucesso!'),
          backgroundColor: Colors.green,
        ),
      );
      Navigator.pushReplacement(
        context,
        MaterialPageRoute(
          builder: (context) => OpportunityDetailsScreen(opportunity: updatedOpportunity),
        ),
      );
    } else {
      ScaffoldMessenger.of(context).showSnackBar(
        const SnackBar(
          content: Text('Falha ao gravar as alterações. Tente novamente!'),
          backgroundColor: Colors.red,
        ),
      );
    }
  }

  @override
  Widget build(BuildContext context) {
    return Scaffold(
      appBar: CustomAppBar(),
      endDrawer: CustomDrawer(),
      body: LayoutBuilder(
        builder: (context, constraints) {
          if (constraints.maxWidth < 600) {
            return _buildMobileLayout();
          } else if (constraints.maxWidth < 1200) {
            return _buildTabletLayout();
          } else {
            return _buildDesktopLayout();
          }
        },
      ),
    );
  }

  Widget _buildMobileLayout() {
    return SingleChildScrollView(
      controller: verticalScrollController,
      padding: const EdgeInsets.all(20.0),
      child: Column(
        crossAxisAlignment: CrossAxisAlignment.start,
        children: [
          Text(
            "Editar a sua Oportunidade:",
            style: TextStyle(fontSize: 30, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 20),
          DynamicTextField(
            label: 'Nome',
            controller: nameController,
          ),
          SizedBox(height: 20),
          DynamicTextField(
            label: 'Descrição',
            controller: descriptionController,
            maxLines: 3,
          ),
          SizedBox(height: 20),
          DynamicTextField(
            label: 'Endereço',
            controller: addressController,
          ),
          SizedBox(height: 20),
          Text(
            'Localização',
            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 10),
          DropdownButton<Location>(
            isExpanded: true,
            value: selectedLocation,
            onChanged: (Location? newValue) {
              setState(() {
                selectedLocation = newValue!;
              });
            },
            items: Location.values.map((Location location) {
              return DropdownMenuItem<Location>(
                value: location,
                child: Text(location.name),
              );
            }).toList(),
          ),
          SizedBox(height: 20),
          Text(
            'Categoria',
            style: TextStyle(fontSize: 16, fontWeight: FontWeight.bold),
          ),
          SizedBox(height: 10),
          DropdownButton<OppCategory>(
            isExpanded: true,
            value: selectedCategory,
            onChanged: (OppCategory? newValue) {
              setState(() {
                selectedCategory = newValue!;
              });
            },
            items: OppCategory.values.map((OppCategory category) {
              return DropdownMenuItem<OppCategory>(
                value: category,
                child: Text(category.name),
              );
            }).toList(),
          ),
          SizedBox(height: 20),
          DynamicTextField(
            label: 'Vagas',
            controller: vacanciesController,
            maxLines: 1,
            inputType: TextInputType.number, // For numeric input
          ),
          SizedBox(height: 20),
          DynamicTextField(
            label: 'Preço (irá ser imposta posteriormente a taxa da plataforma)',
            controller: priceController,
            maxLines: 1,
            inputType: TextInputType.number, // For numeric input
          ),
          SizedBox(height: 20),
          OpportunityDateAndTimePicker(
            initialDate: _oppDateController,
            initialTime: _oppTimeController,
            onDateTimeSelected: (DateTime date) {
              setState(() {
                _oppDateController = date;
              });
            },
            onTimeSelected: (TimeOfDay time) {
              setState(() {
                _oppTimeController = time;
              });
            },
          ),
          SizedBox(height: 20),
          OpportunityImagePicker(
            opportunityImgs: _opportunityImgs,
            onImagesChanged: (newImages) {
              setState(() {
                _opportunityImgs = newImages;
              });
            },
          ),
          SizedBox(height: 20),
          ElevatedButton(
            style: ElevatedButton.styleFrom(
              backgroundColor: Color(0xFF50C878), // Set button color here
            ),
            onPressed: () {
              _saveOpportunity(); // Call the save function
              Navigator.push(
                context,
                MaterialPageRoute(
                  builder: (context) => OpportunityManagerScreen()
                ),
              );
            },
            child: Text(
              'Guardar',
              style: TextStyle(color: Colors.white), // Adjust text color if needed
            ),
          ),
        ],
      ),
    );
  }

  Widget _buildTabletLayout() {
    return _buildMobileLayout();
  }

  Widget _buildDesktopLayout() {
    return _buildMobileLayout();
  }

}





